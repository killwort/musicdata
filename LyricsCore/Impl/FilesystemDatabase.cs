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
        public override IEnumerable<WithCertainity<T>> Get<T>(Song song)
        {
            string dir = Path.Combine(DatabasePath, MakePathSafe(song.Artist), MakePathSafe(song.Title));
            if (!Directory.Exists(dir)) yield break;
            foreach (var f in Directory.EnumerateFiles(dir,"*."+typeof(T).Name))
            {
                var m = new T()
                {
                    OriginalMetadata = song
                };
                using (var stream = File.OpenRead(f))
                    m.Deserialize(stream);
               
                yield return new WithCertainity<T>(m);
            }
        }

        public override void Save<T>(Song song, IEnumerable<WithCertainity<T>> lyrics)
        {
            string dir = Path.Combine(DatabasePath, MakePathSafe(song.Artist), MakePathSafe(song.Title));
            if (!Directory.Exists(dir))Directory.CreateDirectory(dir);
            foreach (var lyric in lyrics)
            {
                var file=Path.Combine(dir,lyric.Value.Hash + "."+typeof(T).Name);
                using (var f = File.Create(file))
                    lyric.Value.Serialize(f);
            }
        }
    }
}
