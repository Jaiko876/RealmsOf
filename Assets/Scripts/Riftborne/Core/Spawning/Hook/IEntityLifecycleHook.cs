using Riftborne.Core.Model;

namespace Riftborne.Core.Spawning
{
    public interface IEntityLifecycleHook
    {
        // Вызывается после создания entity в GameState, но ДО backend.Spawn
        void OnBeforeSpawn(GameEntityId id, string prefabKey, float x, float y);

        // Вызывается после backend.Spawn (когда GO уже существует)
        void OnAfterSpawn(GameEntityId id, string prefabKey, float x, float y);

        // Вызывается ДО backend.Despawn (когда GO ещё жив)
        void OnBeforeDespawn(GameEntityId id);

        // Вызывается после backend.Despawn (когда GO уже уничтожен/деактивирован)
        void OnAfterDespawn(GameEntityId id);
    }

    public abstract class EntityLifecycleHookBase : IEntityLifecycleHook
    {
        public virtual void OnBeforeSpawn(GameEntityId id, string prefabKey, float x, float y) { }
        public virtual void OnAfterSpawn(GameEntityId id, string prefabKey, float x, float y) { }
        public virtual void OnBeforeDespawn(GameEntityId id) { }
        public virtual void OnAfterDespawn(GameEntityId id) { }
    }
}