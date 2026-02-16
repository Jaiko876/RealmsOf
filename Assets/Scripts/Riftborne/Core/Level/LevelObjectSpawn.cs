namespace Riftborne.Core.Level
{
    public readonly struct LevelObjectSpawn
    {
        public readonly string Type;
        public readonly int X;
        public readonly int Y;

        public LevelObjectSpawn(string type, int x, int y)
        {
            Type = type;
            X = x;
            Y = y;
        }
    }
}
