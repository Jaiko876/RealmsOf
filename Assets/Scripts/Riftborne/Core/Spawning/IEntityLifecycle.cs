using Riftborne.Core.Model;

namespace Riftborne.Core.Spawning
{
    public interface IEntityLifecycle
    {
        GameEntityId Spawn(string prefabKey, float x, float y, GameEntityId? fixedId);
        void Despawn(GameEntityId id);
    }
}