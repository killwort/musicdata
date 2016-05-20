using System;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using log4net;

namespace MusicData.Impl
{
    static class MusicbrainzIdentifier
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MusicbrainzIdentifier));
        static string Base = "http://musicbrainz.org/ws/2/";
        /*public Guid? GetArtistId(string artist)
        {
            
        }
        */
        public static Guid? GetAlbumId(string artist, string album)
        {
            var request =(HttpWebRequest) WebRequest.Create(string.Format("{0}release-group?query=artist:%22{1}%22%20AND%20%22{2}%22~3", Base, Uri.EscapeDataString(artist), Uri.EscapeDataString(StringSanitizer.Sanitize(album))));
            request.UserAgent = "TouchMPC-MusicData/1.0 (killwort@gmail.com)";
            using (var response = request.GetResponse())
            using (var strm = response.GetResponseStream())
            {
                var xdoc=XDocument.Load(strm);
                var elem=xdoc.Descendants(XName.Get("release-group", "http://musicbrainz.org/ns/mmd-2.0#")).FirstOrDefault();
                if (elem != null)
                {
                    Logger.DebugFormat("Musicbrainz release-group id {0}", elem.Attribute("id").Value);
                    return Guid.Parse(elem.Attribute("id").Value);
                }
            }
            return null;
        }
    }
}
