using System;

namespace MusicData
{
    public class Song:EventArgs
    {
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Title { get; set; }
        public string FilesystemPath { get; set; }

        public override string ToString()
        {
            return string.Format("{0} by {1} from {2}", Title, Artist, Album);
        }
    }
}