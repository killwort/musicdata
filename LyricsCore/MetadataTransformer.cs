using System.Collections.Generic;

namespace LyricsCore
{
    public abstract class MetadataTransformer
    {
        public abstract IEnumerable<WithCertainity<Song>> GetSynonims(Song source);
    }
}