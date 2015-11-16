using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace LyricsCore.Impl
{
    public class LyricWikiLyricFetcher:HtmlLyricFetcher
    {
        static readonly Uri Base = new Uri("http://lyrics.wikia.com/");
        protected override Uri SearchPage(Song song)
        {
            return new Uri(Base,string.Format("/api.php?artist={0}&song={1}",Uri.EscapeDataString(song.Artist),Uri.EscapeDataString(song.Title)));
        }

        protected override Uri NextSearchPage(Song song, HtmlDocument document)
        {
            return null;
        }

        protected override IEnumerable<WithCertainity<Uri>> ParseSearchResults(Song song, HtmlDocument document)
        {
            if(document.DocumentNode.SelectSingleNode("//pre[contains(text(),\"Not found\")]")!=null)yield break;
            var nodes = document.DocumentNode.SelectNodes("//a[@title=\"url\"]");
            if(nodes==null)yield break;
            
            foreach (var node in nodes)
            {
                yield return new WithCertainity<Uri>(new Uri(node.Attributes["href"].Value),.99f);
            }
        }

        protected override Lyric ParseSongPage(Song song, HtmlDocument document, string hash)
        {
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
