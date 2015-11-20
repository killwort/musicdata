using System.Collections.Generic;

namespace MusicData.Impl
{
    public class NullMetadataTransformer : MetadataTransformer {
        public override IEnumerable<WithCertainity<Song>> GetSynonims(Song source)
        {
            yield return new WithCertainity<Song>(source,1f);
        }
    }
}