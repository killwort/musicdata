using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LyricsCore
{
    public class Lyric:Metadata
    {
        public string Text { get; set; }
        public LyricType Type { get; set; }
        public override void Serialize(Stream stream)
        {
            var bytes = Encoding.UTF8.GetBytes(Text);
            stream.Write(bytes,0,bytes.Length);
        }

        public override void Deserialize(Stream stream)
        {
            using (var reader = new StreamReader(stream))
                Text = reader.ReadToEnd();
        }

        public override string Hash
        {
            get
            {
                var md = MD5.Create();
                return md.ComputeHash(Encoding.UTF8.GetBytes(Text)).Aggregate(new StringBuilder(32), (b, by) => b.Append(by.ToString("x2"))).ToString();
            }
        }
    }
}