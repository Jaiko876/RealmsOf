using System.Collections.Generic;
using Riftborne.Core.Model;

namespace Riftborne.Core.Spawning
{
    public sealed class EntityLifecycle : IEntityLifecycle
    {
        private readonly GameState _state;
        private readonly IEntityIdAllocator _ids;
        private readonly ISpawnBackend _backend;
        private readonly IReadOnlyList<IEntityLifecycleHook> _hooks;

        public EntityLifecycle(
            GameState state,
            IEntityIdAllocator ids,
            ISpawnBackend backend,
            IReadOnlyList<IEntityLifecycleHook> hooks)
        {
            _state = state;
            _ids = ids;
            _backend = backend;
            _hooks = hooks;
        }

        public GameEntityId Spawn(string prefabKey, float x, float y, GameEntityId? fixedId)
        {
            var id = fixedId ?? _ids.Next();

            // 1) Core entity exists
            _state.GetOrCreateEntity(id);

            // 2) hooks (before)
            for (int i = 0; i < _hooks.Count; i++)
                _hooks[i].OnBeforeSpawn(id, prefabKey, x, y);

            // 3) Unity GO
            _backend.Spawn(id, prefabKey, x, y);

            // 4) hooks (after)
            for (int i = 0; i < _hooks.Count; i++)
                _hooks[i].OnAfterSpawn(id, prefabKey, x, y);

            return id;
        }

        public void Despawn(GameEntityId id)
        {
            // 0) hooks (before)
            for (int i = 0; i < _hooks.Count; i++)
                _hooks[i].OnBeforeDespawn(id);

            // 1) kill GO
            _backend.Despawn(id);

            // 2) hooks (after)
            for (int i = 0; i < _hooks.Count; i++)
                _hooks[i].OnAfterDespawn(id);

            // 3) remove entity last
            _state.RemoveEntity(id);
        }
    }
}