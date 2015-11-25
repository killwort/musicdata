using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace MusicData.Impl
{
    public class LyricsComLyricFetcher:HtmlLyricFetcher
    {
        static readonly Uri Base = new Uri("http://lyrics.com/");
        protected override Uri SearchPage(Song song)
        {
            return new Uri(Base,string.Format("http://www.lyrics.com/search.php?keyword={0}%20{1}&what=all&search_btn=Search", Uri.EscapeDataString(song.Artist),Uri.EscapeDataString(song.Title)));
        }

        protected override Uri NextSearchPage(Song song, HtmlDocument document)
        {
            return null;
        }

        protected override IEnumerable<WithCertainity<Uri>> ParseSearchResults(Song song, HtmlDocument document)
        {
            var nodes = document.DocumentNode.SelectNodes("//a[@class=\"lyrics_preview\"]");
            if(nodes==null)yield break;
            
            foreach (var node in nodes)
            {
                yield return new WithCertainity<Uri>(new Uri(Base,node.Attributes["href"].Value),.99f);
            }
        }

        protected override Lyric ParseSongPage(Song song, HtmlDocument document, string hash)
        {
            var box=document.DocumentNode.SelectSingleNode("//div[@id=\"lyrics\"]");
            return new Lyric {Text =HtmlToText(box), Type = LyricType.Simple};
        }
    }
}
