using System.Collections.Generic;

namespace LyricsCore.Impl
{
    public class NullDatabase:Database
    {
        public override IEnumerable<WithCertainity<Lyric>> Get(Song song)
        {
            yield break;
        }

        public override void Save(Song song, IEnumerable<WithCertainity<Lyric>> lyrics)
        {
        }
    }
}
