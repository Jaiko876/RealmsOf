using Riftborne.Core.Model;

namespace Riftborne.Core.Spawning
{
    public interface ISpawnBackend
    {
        void Spawn(GameEntityId id, string prefabKey, float x, float y);
        void Despawn(GameEntityId id);
    }
}