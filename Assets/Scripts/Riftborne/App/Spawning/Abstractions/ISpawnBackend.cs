using Riftborne.Core.Model;

namespace Riftborne.App.Spawning.Abstractions
{
    public interface ISpawnBackend
    {
        void Spawn(GameEntityId id, string prefabKey, float x, float y);
        void Despawn(GameEntityId id);
    }
}