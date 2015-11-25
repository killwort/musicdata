using System.IO;

namespace MusicData
{
    public abstract class Metadata
    {
        public Song OriginalMetadata { get; set; }
        public Song FetchedMetadata { get; set; }
        public string Fetcher { get; set; }
        public string FetcherData { get; set; }

        public abstract void Serialize(Stream stream);
        public abstract void Deserialize(Stream stream);
        public abstract string Hash { get; }
    }
}