using System.Collections.Generic;

namespace LyricsCore.Impl
{
    public class NullFetcher<T> : Fetcher<T> where T:Metadata
    {
        public override IEnumerable<WithCertainity<T>> Fetch(Song song)
        {
            yield break;
        }
    }
}