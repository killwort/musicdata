namespace MusicData
{
    public class SongEventArgs
    {
        public SongEventArgs()
        {
        }

        public SongEventArgs(Song song)
        {
            Song = song;
        }

        public Song Song { get; private set; }
    }
}