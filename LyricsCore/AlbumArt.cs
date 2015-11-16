using System.IO;

namespace LyricsCore
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