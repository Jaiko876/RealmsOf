namespace Riftborne.App.Time.Time
{

    public interface ITickClock
    {
        int CurrentTick { get; }
        void Advance();
    }
}