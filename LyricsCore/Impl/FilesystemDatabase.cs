using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace LyricsCore.Impl
{
    public class FilesystemDatabase:Database
    {
        private static readonly string DatabasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Lyrics","Database");
        private static readonly Regex SmallWordsCleanup = new Regex("((^|\\s+)((\\p{Ll}|\\p{Lu}){1,2}|the|and|feat))+(\\s+|$)");
        private static readonly Regex SymbolsCleanup = new Regex("\\P{Ll}+");

        private static string MakePathSafe(string pathComponent)
        {
            return SymbolsCleanup.Replace(SmallWordsCleanup.Replace(pathComponent.ToLower(), " ").Trim(), "_");
        }
        public override IEnumerable<WithCertainity<Lyric>> Get(Song song)
        {
            string dir = Path.Combine(DatabasePath, MakePathSafe(song.Artist), MakePathSafe(song.Title));
            if (!Directory.Exists(dir)) yield break;
            foreach (var f in Directory.EnumerateFiles(dir))
            {
                yield return new WithCertainity<Lyric>(new Lyric {Text = File.ReadAllText(f), OriginalMetadata = song});
            }
        }

        public override void Save(Song song, IEnumerable<WithCertainity<Lyric>> lyrics)
        {
            var md = MD5.Create();
            string dir = Path.Combine(DatabasePath, MakePathSafe(song.Artist), MakePathSafe(song.Title));
            if (!Directory.Exists(dir))Directory.CreateDirectory(dir);
            foreach (var lyric in lyrics)
            {
                var file=Path.Combine(dir,md.ComputeHash(Encoding.UTF8.GetBytes(lyric.Value.Text)).Aggregate(new StringBuilder(32), (b, by) => b.Append(by.ToString("x2"))) + ".txt");
                using(var f=File.Create(file))
                using (var writer = new StreamWriter(f))
                    writer.Write(lyric.Value.Text);
            }
        }
    }
}
