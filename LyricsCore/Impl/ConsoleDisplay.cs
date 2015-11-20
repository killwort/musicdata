using System;

namespace MusicData.Impl
{
    public class ConsoleDisplay : Display
    {
        public override void DoDisplay(Lyric lyric)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(lyric.OriginalMetadata.Title);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(" by ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(lyric.OriginalMetadata.Artist);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(" from ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(lyric.OriginalMetadata.Album);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(lyric.Text);
            Console.WriteLine();
        }

        public override void DoDisplay(AlbumArt albumArt)
        {
            
        }
    }
}