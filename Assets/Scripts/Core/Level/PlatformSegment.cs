namespace Game.Core.Level
{
    public readonly struct PlatformSegment
    {
        public readonly int StartX;
        public readonly int Length;
        public readonly int Y;

        public PlatformSegment(int startX, int length, int y)
        {
            StartX = startX;
            Length = length;
            Y = y;
        }
    }
}
