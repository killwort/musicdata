using System.Collections.Generic;

namespace LyricsCore
{
    public abstract class Database
    {
        public abstract IEnumerable<WithCertainity<Lyric>> Get(Song song);
        public abstract void Save(Song song, IEnumerable<WithCertainity<Lyric>> lyrics);
    }
}