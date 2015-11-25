using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using log4net;
using Ninject;

namespace MusicData.Impl
{
    public abstract class HtmlLyricFetcher : Fetcher<Lyric>
    {
        [Inject]
        public ILog Logger { get; set; }
        protected abstract Uri SearchPage(Song song);
        protected abstract Uri NextSearchPage(Song song, HtmlDocument document);
        protected abstract IEnumerable<WithCertainity<Uri>> ParseSearchResults(Song song, HtmlDocument document);
        protected abstract Lyric ParseSongPage(Song song, HtmlDocument document,string hash);
        public override IEnumerable<WithCertainity<Lyric>> Fetch(Song song)
        {
            var url = SearchPage(song);
            var data=new StringBuilder();
            do
            {
                Logger.DebugFormat("Fetching search page {0}", url);
                data.Append("Search: ");
                data.AppendLine(url.ToString());
                var request = WebRequest.Create(url);
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream, Encoding(response)))
                {
                    var searchResults = reader.ReadToEnd();
                    var hdoc = new HtmlDocument();
                    hdoc.LoadHtml(searchResults);
                    foreach (var songUrl in ParseSearchResults(song, hdoc))
                    {
                        var lyric = ProcessSongPage(song, songUrl.Value);
                        lyric.FetcherData = data.ToString() + "Page: " + songUrl.Value.ToString();
                        yield return new WithCertainity<Lyric>(lyric,songUrl.Certainity);
                    }
                    url = NextSearchPage(song, hdoc);
                }
            } while (url != null);
        }

        private Lyric ProcessSongPage(Song song, Uri songUrl)
        {
            Logger.DebugFormat("Fetching lyrics page {0}", songUrl);
            var request = WebRequest.Create(songUrl);
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream, Encoding(response)))
            {
                var searchResults = reader.ReadToEnd();
                var hdoc = new HtmlDocument();
                hdoc.LoadHtml(searchResults);
                return ParseSongPage(song, hdoc, songUrl.Fragment);
            }
        }

        private Encoding Encoding(WebResponse enc)
        {
            
            if (string.IsNullOrEmpty(enc.ContentType)) return System.Text.Encoding.UTF8;
            if (enc.ContentType.IndexOf("charset=") == -1) return System.Text.Encoding.UTF8;
            var e=enc.ContentType.Substring(enc.ContentType.IndexOf("charset=") + 8);
            if (e.IndexOf(';') != -1) e = e.Substring(0, e.IndexOf(';'));
            try
            {
                return System.Text.Encoding.GetEncoding(e);
            }
            catch
            {
                return System.Text.Encoding.UTF8;
            }
        }

        protected string HtmlToText(IEnumerable<HtmlNode> nodes)
        {
            var builder = new StringBuilder();
            foreach (var node in nodes)
            {
                WalkHtmlTree(builder, node);
            }
            return Cleanup(builder.ToString());
        }

        private string Cleanup(string s)
        {
            s = s.Trim().Replace("\r","");
            
            var minNewlines = int.MaxValue;
            var maxNewlines = 0;
            var newlines = 0;
            for (var i = 0; i < s.Length; i++)
            {
                if (s[i] == '\n')
                {
                    newlines++;
                }
                else
                {
                    if (newlines > 0)
                    {
                        if (minNewlines > newlines) minNewlines = newlines;
                        if (maxNewlines < newlines) maxNewlines = newlines;
                    }
                    newlines = 0;
                }
            }
            if (minNewlines != int.MaxValue)
            {
                s = s.Replace("\n", ">NL>");
                s = new Regex("(>NL>){" + (minNewlines + 1) + ",}", RegexOptions.Multiline).Replace(s, "\n\n");
                s = new Regex("(>NL>)+", RegexOptions.Multiline).Replace(s, "\n");
            }
            return s;
        }

        protected string HtmlToText(HtmlNode node)
        {
            var builder = new StringBuilder();
            WalkHtmlTree(builder, node);
            return Cleanup(builder.ToString());
        }

        private void WalkHtmlTree(StringBuilder builder, HtmlNode node)
        {
            if (node.NodeType == HtmlNodeType.Comment) return;
            if (node.NodeType == HtmlNodeType.Element)
            {
                var tagName = node.Name.ToLower();
                if(tagName=="script")return;
                if (tagName == "p" || tagName == "br")
                    builder.AppendLine();
            }else if (node.NodeType == HtmlNodeType.Text)
                builder.Append(HttpUtility.HtmlDecode(node.InnerText));
            foreach (var childNode in node.ChildNodes)
            {
                WalkHtmlTree(builder,childNode);
            }
        }
    }
}