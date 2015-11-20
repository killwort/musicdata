using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MusicData.Impl
{

    public class DiscogsAlbumArtFetcher : Fetcher<AlbumArt>
    {
        const string ApiKey = "ZdsLbyZrfbCPWmSaHWpu";
        const string ApiSecret = "geEfbzbuKNRHodvwgiAKSfgMwkuvfxfA";
        const string ApiUrl = "https://api.discogs.com/database/search?type=release&q={0}&artist={1}";
        public override IEnumerable<WithCertainity<AlbumArt>> Fetch(Song song)
        {
            var request = (HttpWebRequest)WebRequest.Create(string.Format(ApiUrl, Uri.EscapeDataString(song.Album), Uri.EscapeDataString(song.Artist)));
            request.UserAgent = "TouchMPC/Album art fetcher";
            request.Headers.Add(HttpRequestHeader.Authorization, string.Format("Discogs key={0}, secret={1}", ApiKey, ApiSecret));
            using (var resp = request.GetResponse())
            using (var strm = resp.GetResponseStream())
            using (var reader = new StreamReader(strm))
            {
                var json = (JObject)JsonConvert.DeserializeObject(reader.ReadToEnd());
                var url = (json["results"]).Children().First()["resource_url"];
                request = (HttpWebRequest)WebRequest.Create(url.ToString());
                request.UserAgent = "TouchMPC/Album art fetcher";
                request.Headers.Add(HttpRequestHeader.Authorization, string.Format("Discogs key={0}, secret={1}", ApiKey, ApiSecret));
                using (var resp2 = request.GetResponse())
                using (var strm2 = resp2.GetResponseStream())
                using (var reader2 = new StreamReader(strm2))
                {
                    json = (JObject)JsonConvert.DeserializeObject(reader2.ReadToEnd());
                    url = (json["images"]).Children().First()["resource_url"];
                    request = (HttpWebRequest)WebRequest.Create(url.ToString());
                    request.UserAgent = "TouchMPC/Album art fetcher";
                    request.Headers.Add(HttpRequestHeader.Authorization, string.Format("Discogs key={0}, secret={1}", ApiKey, ApiSecret));
                    using (var resp3 = request.GetResponse())
                    using (var strm3 = resp3.GetResponseStream())
                    {
                        var ms = new MemoryStream();
                        strm3.CopyTo(ms);
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