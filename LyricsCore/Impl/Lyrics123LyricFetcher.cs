using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace LyricsCore.Impl
{
    public class Lyrics123LyricFetcher:HtmlLyricFetcher
    {
        static readonly Uri Base = new Uri("http://lyrics123.net/");
        static Regex BadChars=new Regex("[^a-z0-9A-Z]+");
        protected override Uri SearchPage(Song song)
        {
            return new Uri(Base,string.Format("/lyrics-search/{0}-{1}/",BadChars.Replace(song.Artist,"-"), BadChars.Replace(song.Title, "-")));
        }

        protected override Uri NextSearchPage(Song song, HtmlDocument document)
        {
            var nodes = document.DocumentNode.SelectNodes("//div[@id=\"b\"]/p[2]//a");
            if (nodes == null||!nodes.Any()) return null;
            return new Uri(Base, nodes.First().Attributes["href"].Value);
        }

        protected override IEnumerable<WithCertainity<Uri>> ParseSearchResults(Song song, HtmlDocument document)
        {
            if(document.DocumentNode.SelectSingleNode("//p[contains(text(),\"Currently we do not have lyrics\")]") !=null)yield break;
            var nodes = document.DocumentNode.SelectNodes("//div[@id=\"b\"]/p[1]//a");
            if(nodes==null)yield break;
            var artist = false;
            foreach (var node in nodes)
            {
                artist = !artist;
                if (artist) continue;
                yield return new WithCertainity<Uri>(new Uri(Base,node.Attributes["href"].Value),.8f);
                yield break;
            }
        }

        protected override Lyric ParseSongPage(Song song, HtmlDocument document, string hash)
        {
            var box=document.DocumentNode.SelectSingleNode("//p[@style=\"padding:5px 0;\"]");
            return new Lyric {Text = HtmlToText(box), Type = LyricType.Simple};
        }
    }
}
