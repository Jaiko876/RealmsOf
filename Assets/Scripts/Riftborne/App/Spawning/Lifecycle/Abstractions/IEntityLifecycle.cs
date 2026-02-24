using Riftborne.Core.Model;

namespace Riftborne.App.Spawning.Lifecycle.Abstractions
{
    public interface IEntityLifecycle
    {
        GameEntityId Spawn(string prefabKey, float x, float y, GameEntityId? fixedId);
        void Despawn(GameEntityId id);
    }
}