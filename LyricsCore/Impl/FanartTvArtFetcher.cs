﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject.Activation.Strategies;

namespace MusicData.Impl
{
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
                            ImageData = ms.ToArray(),
                            FetcherData = iid+"\n"+url.ToString()
                        }, 1f);
                    }
                }
            }
            yield break;
        }
    }
}