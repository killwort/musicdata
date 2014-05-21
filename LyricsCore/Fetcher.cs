using System.Collections.Generic;

namespace LyricsCore
{
    public abstract class Fetcher
    {
        public abstract IEnumerable<WithCertainity<Lyric>> GetLyrics(Song song);
    }
}