using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LyricsCore.Impl
{
    public class MpdFilesystemArtFetcher : Fetcher<AlbumArt>
    {
        private string MpdMusicBase = @"\\SRV\public\music";
        private string[] AcceptableExtensions = new[] {".jpg", ".png", ".gif", ".bmp", "*.jpeg"};
        public override IEnumerable<WithCertainity<AlbumArt>> Fetch(Song song)
        {
            var dir = Path.GetDirectoryName(Path.Combine(MpdMusicBase, song.FilesystemPath));
            if (Directory.Exists(dir))
            {
                var candidates = Directory.EnumerateFiles(dir)
                        .Where(file => AcceptableExtensions.Contains(Path.GetExtension(file).ToLower()))
                        .Select(x=>new FileInfo(x))
                        .Where(x=>x.Length<20971520)//No more than 20M
                        .OrderByDescending(x=>x.Length).ToList();
                if (candidates.Any())
                {
                    var norm = candidates.First().Length;
                    foreach (var candidate in candidates)
                    {
                        yield return new WithCertainity<AlbumArt>(new AlbumArt(candidate.OpenRead()),((float)candidate.Length)/norm);
                    }
                }
            }
            yield break;
        }
    }
    public class FanartTvArtFetcher : Fetcher<AlbumArt>
    {
        const string ApiKey = "08521997c1955a6c6f2f5b8c75fffd4c";
        public override IEnumerable<WithCertainity<AlbumArt>> Fetch(Song song)
        {
            var iid = MusicbrainzIdentifier.GetAlbumId(song.Artist, song.Album);
            if (iid.HasValue)
            {
                var request = WebRequest.Create(string.Format("http://webservice.fanart.tv/v3/music/albums/{0}?api_key={1}", iid.Value.ToString("D"), ApiKey));
                using (var resp = request.GetResponse())
                using (var strm = resp.GetResponseStream())
                using (var reader = new StreamReader(strm))
                {
                    var json = (JObject)JsonConvert.DeserializeObject(reader.ReadToEnd());
                    var url = ((JProperty)(json["albums"]).Children().First()).Value["albumcover"].First()["url"];
                    using (var resp2 = WebRequest.Create(url.ToString()).GetResponse())
                    using (var strm2 = resp2.GetResponseStream())
                    {
                        var ms=new MemoryStream();
                        strm2.CopyTo(ms);
                        yield return new WithCertainity<AlbumArt>(new AlbumArt
                        {
                            ImageData = ms.ToArray()
                        }, 1f);
                    }
                }
            }
            yield break;
        }
    }
}