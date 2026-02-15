namespace Game.Core.Spawning
{
    public readonly struct SpawnRequest
    {
        public readonly string Type;
        public readonly int X;
        public readonly int Y;

        public SpawnRequest(string type, int x, int y)
        {
            Type = type;
            X = x;
            Y = y;
        }
    }
}
