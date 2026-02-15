namespace Game.Core.Level
{
    public readonly struct LevelTile
    {
        public readonly int X;
        public readonly int Y;
        public readonly LevelTileKind Kind;

        public LevelTile(int x, int y, LevelTileKind kind)
        {
            X = x;
            Y = y;
            Kind = kind;
        }
    }
}
