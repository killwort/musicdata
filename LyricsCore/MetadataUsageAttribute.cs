using System;

namespace MusicData
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class MetadataUsageAttribute : Attribute
    {
        public Usage Usage { get; private set; }

        // This is a positional argument
        public MetadataUsageAttribute(Usage usage)
        {
            Usage = usage;
        }

    }
}