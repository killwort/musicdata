using System.Text.RegularExpressions;

namespace MusicData.Impl
{
    static class StringSanitizer
    {
        private static readonly Regex SmallWordsCleanup = new Regex("((^|\\s+)((\\p{Ll}|\\p{Lu}){1,2}|the|and|feat|\\([^)]+\\)))+(\\s+|$)");

        public static string Sanitize(string src)
        {
            return SmallWordsCleanup.Replace(src, " ").Trim();
        }
    }
}
