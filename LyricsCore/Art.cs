using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LyricsCore
{
    public abstract class Art:Metadata
    {
        public Art() { }

        public Art(Stream input)
        {
            ImageData=new byte[input.Length];
            input.Read(ImageData, 0, ImageData.Length);
            input.Close();
            input.Dispose();
        }
        public byte[] ImageData { get; set; }
        public override void Serialize(Stream stream)
        {
            stream.Write(ImageData,0,ImageData.Length);
        }

        public override void Deserialize(Stream stream)
        {
            var ms=new MemoryStream();
            stream.CopyTo(ms);
            ImageData = ms.ToArray();
        }

        public override string Hash
        {
            get
            {
                var md = MD5.Create();
                return md.ComputeHash(ImageData).Aggregate(new StringBuilder(32), (b, by) => b.Append(by.ToString("x2"))).ToString();
            }
        }
    }
}