namespace Riftborne.App.Time
{

    public interface ITickClock
    {
        int CurrentTick { get; }
        void Advance();
    }
}