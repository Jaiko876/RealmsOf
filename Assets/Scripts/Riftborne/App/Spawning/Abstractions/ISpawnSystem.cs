namespace Riftborne.App.Spawning.Abstractions
{
    public interface ISpawnSystem
    {
        void Spawn(in SpawnRequest request);
    }
}
