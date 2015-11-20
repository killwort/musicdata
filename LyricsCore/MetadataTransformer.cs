using System.Collections.Generic;

namespace MusicData
{
    public abstract class MetadataTransformer
    {
        public abstract IEnumerable<WithCertainity<Song>> GetSynonims(Song source);
    }
}