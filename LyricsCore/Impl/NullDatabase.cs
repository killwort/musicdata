using System.Collections.Generic;

namespace LyricsCore.Impl
{
    public class NullDatabase:Database
    {
        public override IEnumerable<WithCertainity<T>> Get<T>(Song song)
        {
            yield break;
        }

        public override void Save<T>(Song song, IEnumerable<WithCertainity<T>> lyrics)
        {
        }
    }
}
