using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using log4net;
using Ninject;

namespace LyricsCore.Impl
{
    public class DarkLyricsLyricFetcher : HtmlLyricFetcher
    {
        private static readonly Uri Base = new Uri("http://www.darklyrics.com");
        protected override Uri SearchPage(Song song)
        {
            return new Uri(Base, string.Format("search?q={0}", Uri.EscapeDataString(string.Format("{0} {1}", song.Artist, song.Title))));
        }

        protected override Uri NextSearchPage(Song song, HtmlDocument document)
        {
            var nextLinks = document.DocumentNode.SelectNodes("//a[text()=\"Next\"]");
            if (nextLinks != null && nextLinks.Any())
            {
                return new Uri(Base, HttpUtility.HtmlDecode(nextLinks.First().Attributes["href"].Value));
            }
            return null;
        }

        private static readonly Regex Cleanup = new Regex("(\\s+(\\p{Ll}|\\p{Lu}){1,2}\\s+)|the|and|\\s+|\\.|-");
        protected override IEnumerable<WithCertainity<Uri>> ParseSearchResults(Song song, HtmlDocument document)
        {
            var nodes = document.DocumentNode.SelectNodes("//h3[contains(text(),\"Songs:\")]/following::div[@class=\"sen\"]");
            if (nodes == null)
                yield break;
            foreach (var node in nodes)
            {
                var title = node.SelectSingleNode("h2/a");
                if (title != null)
                {
                    var cleanTitle = Cleanup.Replace(title.InnerText.ToLower(), "");
                    if (cleanTitle.Contains(Cleanup.Replace(song.Artist.ToLower(), "")) && cleanTitle.Contains(Cleanup.Replace(song.Title.ToLower(), "")))
                        yield return new WithCertainity<Uri>(new Uri(Base, HttpUtility.HtmlDecode(title.Attributes["href"].Value)), 0.99f);
                }
            }
        }

        protected override Lyric ParseSongPage(Song song, HtmlDocument document, string hash)
        {
            var songTitleNode = document.DocumentNode.SelectSingleNode("//h3/a[@name=\"" + hash.Substring(1) + "\"]/..");
            var nextTitleNode = songTitleNode.SelectSingleNode("following-sibling::h3[1]") 
                ?? songTitleNode.SelectSingleNode("following-sibling::div[@class=\"thanks\"]");
            var songNodes = new List<HtmlNode>();
            songTitleNode = songTitleNode.NextSibling;
            while (songTitleNode != null && songTitleNode != nextTitleNode)
            {
                songNodes.Add(songTitleNode);
                songTitleNode = songTitleNode.NextSibling;
            }
            return new Lyric
            {
                Text = HtmlToText(songNodes)
            };
        }
    }
}
