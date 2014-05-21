namespace LyricsCore.Configuration
{
    public class InjectableSetting<T>
    {
        public bool HasValue { get; private set; }
        public InjectableSetting(T value)
        {
            Value = value;
            HasValue = true;
        }

        public InjectableSetting()
        {
            HasValue = false;
        }

        public T ValueOrDefault(T _default)
        {
            return HasValue ? Value : _default;
        }
        public T Value { get; private set; }
    }
}