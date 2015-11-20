using System;

namespace MusicData
{
    [Flags]
    public enum Usage
    {
        Track=1,
        Album=2,
        Artist=4
    }
}