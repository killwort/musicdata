using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace MusicData.Impl
{
    public class MpdFilesystemArtFetcher : Fetcher<AlbumArt>
    {
        public MpdFilesystemArtFetcher()
        {
            var asr=new AppSettingsReader();
            try
            {
                MpdMusicBase = (string) asr.GetValue(typeof (MpdFilesystemArtFetcher).FullName + ".MpdMusicBase", typeof (string));
            }
            catch
            {
                MpdMusicBase = null;
            }
        }
        private string MpdMusicBase;
        private string[] AcceptableExtensions = new[] {".jpg", ".png", ".gif", ".bmp", "*.jpeg"};
        public override IEnumerable<WithCertainity<AlbumArt>> Fetch(Song song)
        {
            if(string.IsNullOrEmpty(MpdMusicBase))yield break;
            var dir = Path.GetDirectoryName(Path.Combine(MpdMusicBase, song.FilesystemPath));
            if (Directory.Exists(dir))
            {
                var candidates = Directory.EnumerateFiles(dir)
                    .Where(file => AcceptableExtensions.Contains(Path.GetExtension(file).ToLower()))
                    .Select(x=>new FileInfo(x))
                    .Where(x=>x.Length<20971520)//No more than 20M
                    .OrderByDescending(x=>x.Length).ToList();
                if (candidates.Any())
                {
                    var norm = candidates.First().Length;
                    foreach (var candidate in candidates)
                    {
                        yield return new WithCertainity<AlbumArt>(new AlbumArt(candidate.OpenRead()) {FetcherData = candidate.FullName}, ((float) candidate.Length)/norm);
                    }
                }
            }
            yield break;
        }
    }
}