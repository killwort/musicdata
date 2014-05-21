namespace LyricsCore
{
    public abstract class PlayerInteraction
    {
        public event SongEvent SongChanged;

        protected virtual void OnSongChanged(SongEventArgs args)
        {
            SongEvent handler = SongChanged;
            if (handler != null) handler(args);
        }
    }
}