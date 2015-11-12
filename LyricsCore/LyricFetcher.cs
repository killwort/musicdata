using System.Collections.Generic;

namespace LyricsCore
{
    public abstract class LyricFetcher
    {
        public abstract IEnumerable<WithCertainity<Lyric>> GetLyrics(Song song);
    }

    public abstract class ArtFetcher
    {
        public abstract IEnumerable<WithCertainity<Art>> GetArt(Song song);
    }
}