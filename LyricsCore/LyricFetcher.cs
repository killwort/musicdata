using System.Collections.Generic;

namespace MusicData
{
    public abstract class Fetcher<T> where T:Metadata
    {
        public abstract IEnumerable<WithCertainity<T>> Fetch(Song song);
    }
}