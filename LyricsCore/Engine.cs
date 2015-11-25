using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;

namespace MusicData
{
    public class Engine
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Engine));
        private readonly IEnumerable<Fetcher<Lyric>> _lyricFetchers;
        private readonly IEnumerable<Fetcher<AlbumArt>> _artFetchers;
        private readonly Database _database;
        private readonly PlayerInteraction _player;
        private readonly List<MetadataTransformer> _metadataTransformers;
        private readonly Display _display;
        private readonly ILog _logger;

        public Engine(IEnumerable<Fetcher<Lyric>> fetchers,
            IEnumerable<Fetcher<AlbumArt>> artFetchers,
            Database database, PlayerInteraction player, IEnumerable<MetadataTransformer> metadataTransformers,
            Display display, ILog logger)
        {
            _lyricFetchers = fetchers;
            _artFetchers = artFetchers;
            _database = database;
            _player = player;
            _display = display;
            _logger = logger;
            _metadataTransformers = new List<MetadataTransformer>(metadataTransformers);
            _player.SongChanged += Player_SongChanged;
        }

        void Player_SongChanged(object sender, Song song)
        {
            Logger.InfoFormat("Going to fetch metadata for {0}", song);
            Task.Factory.StartNew(() =>
            {
                var lyric = FetchLyrics(song, false);
                if (lyric.Any())
                    _display.DoDisplay(lyric.First().Value);
                else
                    _display.DoDisplay(new Lyric {Text = "", OriginalMetadata = song});
            });
            Task.Factory.StartNew(() =>
            {
                var lyric = FetchAlbumArt(song, true);
                if (lyric.Any())
                    _display.DoDisplay(lyric.First().Value);
                else
                    _display.DoDisplay(new AlbumArt {ImageData = null, OriginalMetadata = song});
            });
        }

        private IEnumerable<WithCertainity<T>> Fetch<T>(Song song, bool stopOnFirst,IEnumerable<Fetcher<T>> fetchers) where T :Metadata, new()
        {
            if (string.IsNullOrEmpty(song.Artist) && string.IsNullOrEmpty(song.Title)) return new WithCertainity<T>[0];
            if (string.IsNullOrEmpty(song.Artist))
            {
                var i = song.Title.IndexOf('-');
                if (i != -1)
                {
                    song.Artist = song.Title.Substring(0, i).Trim();
                    song.Title = song.Title.Substring(i + 1).Trim();
                }
            }
            var cached = _database.Get<T>(song);
            if (cached.Any())
                return cached;
            var synonims = _metadataTransformers.Any() ? _metadataTransformers.SelectMany(x => x.GetSynonims(song)).OrderByDescending(x => x.Certainity).ToArray() : new[] { new WithCertainity<Song>(song, 1f) };
            var results = new List<WithCertainity<T>>();
            Parallel.ForEach(fetchers, (fetcher, state) =>
            {
                Parallel.ForEach(synonims, (synonim, innerState) =>
                {
                    WithCertainity<T>[] result;
                    try
                    {
                        result = fetcher.Fetch(synonim.Value).ToArray();
                    }
                    catch (Exception e)
                    {
                        _logger.Error(string.Format("Error fetching metadata for {0}:{1}:{2} with fetcher {3}", synonim.Value.Artist, synonim.Value.Album, synonim.Value.Title, fetcher.GetType().Name), e);
                        return;
                    }
                    if (result.Any())
                    {
                        results.AddRange(result.Select(x =>
                        {
                            x.Value.FetchedMetadata = synonim.Value;
                            x.Value.OriginalMetadata = song;
                            x.Certainity *= synonim.Certainity;
                            x.Value.Fetcher = fetcher.GetType().FullName;
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
            _database.Save(song, results);
            return results.OrderByDescending(x => x.Certainity);
        }

        private IEnumerable<WithCertainity<AlbumArt>> FetchAlbumArt(Song song, bool stopOnFirst)
        {
            return Fetch(song, stopOnFirst, _artFetchers);
        }

        private IEnumerable<WithCertainity<Lyric>> FetchLyrics(Song song, bool stopOnFirst)
        {
            return Fetch(song, stopOnFirst, _lyricFetchers);
        }
    }
}
