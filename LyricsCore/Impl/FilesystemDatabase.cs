using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LyricsCore.Impl
{
    public class FilesystemDatabase:Database
    {
        private static readonly string DatabasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MusicData","Database");
        private static readonly Regex SymbolsCleanup = new Regex("\\P{Ll}+");

        private static string MakePathSafe(string pathComponent)
        {
            return SymbolsCleanup.Replace(StringSanitizer.Sanitize(pathComponent.ToLower()), "_");
        }

        private string GetBasePath<T>(Song song)
        {
            var usage=Usage.Track;
            var attr=typeof (T).GetCustomAttributes(typeof (MetadataUsageAttribute),true).OfType<MetadataUsageAttribute>().FirstOrDefault();
            if (attr != null)
                usage = attr.Usage;
            string path;
            switch (usage)
            {
                case Usage.Artist:
                    path = Path.Combine(DatabasePath, MakePathSafe(song.Artist));
                    break;
                    case Usage.Album:
                    path = Path.Combine(DatabasePath, MakePathSafe(song.Artist), "_Albums", MakePathSafe(song.Album));
                    break;
                case Usage.Track:
                default:
                    path= Path.Combine(DatabasePath, MakePathSafe(song.Artist), MakePathSafe(song.Title));
                    break;
            }
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
        public override IEnumerable<WithCertainity<T>> Get<T>(Song song)
        {
            string dir = GetBasePath<T>(song);
            foreach (var f in Directory.EnumerateFiles(dir,"*."+typeof(T).Name))
            {
                var m = new T
                {
                    OriginalMetadata = song
                };
                using (var stream = File.OpenRead(f))
                    m.Deserialize(stream);
               
                yield return new WithCertainity<T>(m);
            }
        }

        public override void Save<T>(Song song, IEnumerable<WithCertainity<T>> data)
        {
            string dir = GetBasePath<T>(song);
            foreach (var lyric in data)
            {
                var file=Path.Combine(dir,lyric.Value.Hash + "."+typeof(T).Name);
                using (var f = File.Create(file))
                    lyric.Value.Serialize(f);
            }
        }
    }
}
