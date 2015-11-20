using System;

namespace MusicData
{
    public abstract class PlayerInteraction
    {
        public event EventHandler<Song> SongChanged;

        protected virtual void OnSongChanged(Song args)
        {
            var handler = SongChanged;
            if (handler != null) handler(this,args);
        }
    }
}