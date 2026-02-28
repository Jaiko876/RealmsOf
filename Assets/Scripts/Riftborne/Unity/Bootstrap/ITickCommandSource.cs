namespace Riftborne.Unity.Bootstrap
{
    public interface ITickCommandSource
    {
        void ProduceCommandsForTick(int tick);
    }
}