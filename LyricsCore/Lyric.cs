namespace LyricsCore
{
    public class Lyric
    {
        public Song OriginalMetadata { get; set; }
        public Song FetchedMetadata { get; set; }
        public string Text { get; set; }
        public LyricType Type { get; set; }
    }
}