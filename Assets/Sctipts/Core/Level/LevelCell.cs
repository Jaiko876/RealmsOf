namespace Game.Core.Level
{
    public readonly struct LevelCell
    {
        public readonly int X;
        public readonly int Y;

        public LevelCell(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
