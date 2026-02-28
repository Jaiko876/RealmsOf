using System;
using System.Collections.Generic;
using Riftborne.App.Spawning.Abstractions;
using Riftborne.App.Spawning.Hooks.Lifecycle;
using Riftborne.App.Spawning.Lifecycle.Abstractions;
using Riftborne.Core.Model;

namespace Riftborne.App.Spawning.Lifecycle
{
    public sealed class EntityLifecycle : IEntityLifecycle
    {
        private readonly GameState _state;
        private readonly IEntityIdAllocator _ids;
        private readonly ISpawnBackend _backend;
        private readonly IEntityLifecycleHook[] _hooksOrdered;

        public EntityLifecycle(
            GameState state,
            IEntityIdAllocator ids,
            ISpawnBackend backend,
            IReadOnlyList<IEntityLifecycleHook> hooks)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _ids = ids ?? throw new ArgumentNullException(nameof(ids));
            _backend = backend ?? throw new ArgumentNullException(nameof(backend));
            if (hooks == null) throw new ArgumentNullException(nameof(hooks));

            _hooksOrdered = OrderHooks(hooks);
        }

        public GameEntityId Spawn(string prefabKey, float x, float y, GameEntityId? fixedId)
        {
            if (string.IsNullOrWhiteSpace(prefabKey))
                throw new ArgumentException("prefabKey is required", nameof(prefabKey));

            var id = fixedId ?? _ids.Next();

            _state.GetOrCreateEntity(id);

            for (int i = 0; i < _hooksOrdered.Length; i++)
                _hooksOrdered[i].OnBeforeSpawn(id, prefabKey, x, y);

            _backend.Spawn(id, prefabKey, x, y);

            for (int i = 0; i < _hooksOrdered.Length; i++)
                _hooksOrdered[i].OnAfterSpawn(id, prefabKey, x, y);

            return id;
        }

        public void Despawn(GameEntityId id)
        {
            for (int i = 0; i < _hooksOrdered.Length; i++)
                _hooksOrdered[i].OnBeforeDespawn(id);

            _backend.Despawn(id);

            for (int i = 0; i < _hooksOrdered.Length; i++)
                _hooksOrdered[i].OnAfterDespawn(id);

            _state.RemoveEntity(id);
        }

        private static IEntityLifecycleHook[] OrderHooks(IReadOnlyList<IEntityLifecycleHook> hooks)
        {
            var list = new List<(int order, IEntityLifecycleHook hook)>(hooks.Count);

            for (int i = 0; i < hooks.Count; i++)
            {
                var h = hooks[i];
                if (h == null) continue;

                var order = 0;
                var ordered = h as IOrderedEntityLifecycleHook;
                if (ordered != null) order = ordered.Order;

                list.Add((order, h));
            }

            list.Sort((a, b) =>
            {
                var c = a.order.CompareTo(b.order);
                return c != 0 ? c : string.CompareOrdinal(a.hook.GetType().FullName, b.hook.GetType().FullName);
            });

            var arr = new IEntityLifecycleHook[list.Count];
            for (int i = 0; i < list.Count; i++)
                arr[i] = list[i].hook;

            return arr;
        }
    }
}