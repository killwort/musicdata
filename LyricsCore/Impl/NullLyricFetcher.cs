using System.Collections.Generic;

namespace MusicData.Impl
{
    public class NullFetcher<T> : Fetcher<T> where T:Metadata
    {
        public override IEnumerable<WithCertainity<T>> Fetch(Song song)
        {
            yield break;
        }
    }
}