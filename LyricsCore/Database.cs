using System.Collections.Generic;

namespace MusicData
{
    public abstract class Database
    {
        public abstract IEnumerable<WithCertainity<T>> Get<T>(Song song) where T:Metadata,new ();
        public abstract void Save<T>(Song song, IEnumerable<WithCertainity<T>> meta) where T : Metadata;
    }
}