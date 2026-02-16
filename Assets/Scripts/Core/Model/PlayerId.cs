namespace Game.Core.Model
{
    public readonly struct PlayerId
    {
        public int Value { get; }

        public PlayerId(int value)
        {
            Value = value;
        }

        public static PlayerId Local => new PlayerId(0);
    }
}
