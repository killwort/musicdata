using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace MusicData.Impl
{
    public class LyricWikiLyricFetcher:HtmlLyricFetcher
    {
        static readonly Uri Base = new Uri("http://lyrics.wikia.com/");
        //http://lyrics.wikia.com/wiki/Special:Search?search=asian+dub+foudatation+tank&fulltext=Search
        protected override Uri SearchPage(Song song)
        {
            return new Uri(Base,string.Format("/wiki/Special:Search?search={0}+{1}&fulltext=Search", Uri.EscapeDataString(song.Artist),Uri.EscapeDataString(song.Title)));
        }

        protected override Uri NextSearchPage(Song song, HtmlDocument document)
        {
            return null;
        }

        protected override IEnumerable<WithCertainity<Uri>> ParseSearchResults(Song song, HtmlDocument document)
        {
            if(document.DocumentNode.SelectSingleNode("//pre[contains(text(),\"Not found\")]")!=null)yield break;
            var nodes = document.DocumentNode.SelectNodes("//article//a[@class=\"result-link\"]");
            if(nodes==null)yield break;
            
            foreach (var node in nodes)
            {
                var parts = node.InnerText.Split(new[] {':'}, 2);
                if (parts.Length == 1) continue;
                if (!StringSanitizer.IsSame(parts[0], song.Artist)) continue;
                if (!StringSanitizer.IsSame(parts[1], song.Title)) continue;

                yield return new WithCertainity<Uri>(new Uri(node.Attributes["href"].Value),.99f);
            }
        }

        protected override Lyric ParseSongPage(Song song, HtmlDocument document, string hash)
        {
            var title= document.DocumentNode.SelectSingleNode("//div[@id=\"song-header-title\"]");
            if (title == null) return null;
            var box=document.DocumentNode.SelectSingleNode("//div[@class=\"lyricbox\"]");
            var toRemove = box.SelectNodes("div[@class=\"rtMatcher\"]");
            if(toRemove!=null)
                foreach (var remove in toRemove)
                {
                    box.ChildNodes.Remove(remove);
                }
            return new Lyric {Text =HtmlToText(box), Type = LyricType.Simple};
        }
    }
}
