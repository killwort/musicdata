namespace LyricsCore
{
    public class WithCertainity<T>
    {
        public WithCertainity(T value, float certainity)
        {
            Value = value;
            Certainity = certainity;
        }

        public WithCertainity(T value)
        {
            Value = value;
        }

        public T Value { get; private set; }
        public float Certainity { get; set; }
    }
}