namespace Riftborne.Core.Model
{
    public readonly struct PlayerId
    {
        public int Value { get; }

        public PlayerId(int value)
        {
            Value = value;
        }
    }
}
