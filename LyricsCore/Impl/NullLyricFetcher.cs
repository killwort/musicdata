using System.Collections.Generic;

namespace LyricsCore.Impl
{
    public class NullLyricFetcher : LyricFetcher
    {
        public override IEnumerable<WithCertainity<Lyric>> GetLyrics(Song song)
        {
            yield break;
        }
    }
}