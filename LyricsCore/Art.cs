using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LyricsCore
{
    public class Art:Metadata
    {
        public byte[] AlbumArt { get; set; }
        public override void Serialize(Stream stream)
        {
            stream.Write(AlbumArt,0,AlbumArt.Length);
        }

        public override void Deserialize(Stream stream)
        {
            var ms=new MemoryStream();
            stream.CopyTo(ms);
            AlbumArt = ms.ToArray();
        }

        public override string Hash
        {
            get
            {
                var md = MD5.Create();
                return md.ComputeHash(AlbumArt).Aggregate(new StringBuilder(32), (b, by) => b.Append(by.ToString("x2"))).ToString();
            }
        }
    }
}