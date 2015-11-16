using System;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace LyricsCore.Impl
{
    static class MusicbrainzIdentifier
    {
        static string Base = "http://musicbrainz.org/ws/2/";
        /*public Guid? GetArtistId(string artist)
        {
            
        }
        */
        public static Guid? GetAlbumId(string artist, string album)
        {
            var request = WebRequest.Create(string.Format("{0}release-group?query=artist:{1}%20AND%20%22{2}%22", Base, Uri.EscapeDataString(artist), Uri.EscapeDataString(album)));
            using (var response = request.GetResponse())
            using (var strm = response.GetResponseStream())
            {
                var xdoc=XDocument.Load(strm);
                var elem=xdoc.Descendants(XName.Get("release-group", "http://musicbrainz.org/ns/mmd-2.0#")).FirstOrDefault();
                if (elem != null)
                {
                    return Guid.Parse(elem.Attribute("id").Value);
                }
            }
            return null;
        }
    }
}
