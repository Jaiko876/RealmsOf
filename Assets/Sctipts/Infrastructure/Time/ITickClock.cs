namespace Game.Infrastructure.Time
{

    public interface ITickClock
    {
        int CurrentTick { get; }
        void Advance();
    }
}