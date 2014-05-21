using System.Collections.Generic;

namespace LyricsCore.Impl
{
    public class NullFetcher : Fetcher
    {
        public override IEnumerable<WithCertainity<Lyric>> GetLyrics(Song song)
        {
            yield break;
        }
    }
}