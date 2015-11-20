using System.IO;

namespace MusicData
{
    [MetadataUsage(Usage.Album)]
    public class AlbumArt : Art
    {
        public AlbumArt()
        {
        }

        public AlbumArt(Stream input) : base(input)
        {
        }
    }
}