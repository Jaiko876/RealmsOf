namespace Game.Infrastructure.Time
{

    public sealed class FixedTickClock : ITickClock
    {
        public int CurrentTick { get; private set; }
        public void Advance() => CurrentTick++;
    }
}