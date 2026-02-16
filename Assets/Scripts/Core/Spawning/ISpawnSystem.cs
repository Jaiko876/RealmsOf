namespace Game.Core.Spawning
{
    public interface ISpawnSystem
    {
        void Spawn(in SpawnRequest request);
    }
}
