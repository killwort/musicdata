using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;

namespace LyricsCore
{
    public class Engine
    {
        private readonly List<Fetcher> _fetchers;
        private readonly Database _database;
        private PlayerInteraction Player;
        private readonly List<MetadataTransformer> _metadataTransformers;
        private readonly Display _display;
        private readonly ILog _logger;

        public Engine(IEnumerable<Fetcher> fetchers, Database database, PlayerInteraction player, IEnumerable<MetadataTransformer> metadataTransformers, Display display, ILog logger)
        {
            _fetchers = new List<Fetcher>(fetchers);
            _database = database;
            Player = player;
            _display = display;
            _logger = logger;
            _metadataTransformers = new List<MetadataTransformer>(metadataTransformers);
            Player.SongChanged += Player_SongChanged;
        }

        void Player_SongChanged(SongEventArgs args)
        {
            var lyric=FetchLyrics(args.Song, true);
            if (lyric.Any())
                _display.DoDisplay(lyric.First().Value);
            else
                _display.DoDisplay(new Lyric {Text = "", OriginalMetadata = args.Song});
        }

        private IEnumerable<WithCertainity<Lyric>> FetchLyrics(Song song, bool stopOnFirst)
        {
            if(string.IsNullOrEmpty(song.Artist)&&string.IsNullOrEmpty(song.Title))return new WithCertainity<Lyric>[0];
            if (string.IsNullOrEmpty(song.Artist))
            {
                var i = song.Title.IndexOf('-');
                if (i != -1)
                {
                    song.Artist = song.Title.Substring(0, i).Trim();
                    song.Title = song.Title.Substring(i + 1).Trim();
                }
            }
            var cached = _database.Get(song);
            if (cached.Any())
                return cached;
            var synonims = _metadataTransformers.Any()?_metadataTransformers.SelectMany(x => x.GetSynonims(song)).OrderByDescending(x=>x.Certainity).ToArray():new[] {new WithCertainity<Song>(song,1f) };
            var results = new List<WithCertainity<Lyric>>();
            Parallel.ForEach(_fetchers, (fetcher, state) =>
            {
                Parallel.ForEach(synonims, (synonim, innerState) =>
                {
                    WithCertainity<Lyric>[] result;
                    try
                    {
                        result = fetcher.GetLyrics(synonim.Value).ToArray();
                    }
                    catch (Exception e)
                    {
                        _logger.Error(string.Format("Error fetching lyrics for {0}:{1}:{2} with fetcher {3}",synonim.Value.Artist,synonim.Value.Album,synonim.Value.Title,fetcher.GetType().Name),e);
                        return;
                    }
                    if (result.Any())
                    {
                        results.AddRange(result.Select(x =>
                        {
                            x.Value.FetchedMetadata = synonim.Value;
                            x.Value.OriginalMetadata = song;
                            x.Certainity *= synonim.Certainity;
                            return x;
                        }));
                        if (stopOnFirst)
                        {
                            innerState.Break();
                            state.Break();
                        }
                    }
                });
            });
            _database.Save(song,results);
            return results.OrderByDescending(x=>x.Certainity);
        }
    }
}
