using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using LyricsCore.Impl;

namespace LyricsCore
{
    public class Engine
    {
        private readonly List<LyricFetcher> _lyricFetchers;
        private readonly List<ArtFetcher> _artFetchers;
        private readonly Database _database;
        private PlayerInteraction Player;
        private readonly List<MetadataTransformer> _metadataTransformers;
        private readonly Display _display;
        private readonly ILog _logger;

        public Engine(IEnumerable<LyricFetcher> fetchers,IEnumerable<ArtFetcher> artFetchers, Database database, PlayerInteraction player, IEnumerable<MetadataTransformer> metadataTransformers, Display display, ILog logger)
        {
            _lyricFetchers = new List<LyricFetcher>(fetchers);
            _artFetchers=new List<ArtFetcher>(artFetchers);
            _database = database;
            Player = player;
            _display = display;
            _logger = logger;
            _metadataTransformers = new List<MetadataTransformer>(metadataTransformers);
            Player.SongChanged += Player_SongChanged;
        }

        void Player_SongChanged(SongEventArgs args)
        {
            Task.Factory.StartNew(() =>
            {
                var lyric = FetchLyrics(args.Song, true);
                if (lyric.Any())
                    _display.DoDisplay(lyric.First().Value);
                else
                    _display.DoDisplay(new Lyric {Text = "", OriginalMetadata = args.Song});
            });
            Task.Factory.StartNew(() =>
            {
                var lyric = FetchAlbumArt(args.Song, true);
                if (lyric.Any())
                    _display.DoDisplay(lyric.First().Value);
                else
                    _display.DoDisplay(new Art {AlbumArt = null, OriginalMetadata = args.Song});
            });
        }

        private IEnumerable<WithCertainity<T>> Fetch<T,F>(Song song, bool stopOnFirst,IEnumerable<F> fetchers,Func<F,Song,IEnumerable<WithCertainity<T>>> xfetcher) where T :Metadata, new()
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
                        result = xfetcher(fetcher,synonim.Value).ToArray();
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

        private IEnumerable<WithCertainity<Art>> FetchAlbumArt(Song song, bool stopOnFirst)
        {
            return Fetch(song, stopOnFirst, _artFetchers, (f, s) => f.GetArt(s));
        }

        private IEnumerable<WithCertainity<Lyric>> FetchLyrics(Song song, bool stopOnFirst)
        {
            return Fetch(song, stopOnFirst, _lyricFetchers, (f, s) => f.GetLyrics(s));
        }
    }
}
