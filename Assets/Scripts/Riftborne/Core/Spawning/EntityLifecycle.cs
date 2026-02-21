using Riftborne.Core.Model;
using Riftborne.Core.Physics.Abstractions;
using Riftborne.Core.Stores;

namespace Riftborne.Core.Spawning
{
    public sealed class EntityLifecycle : IEntityLifecycle
    {
        private readonly GameState _state;
        private readonly IEntityIdAllocator _ids;
        private readonly ISpawnBackend _backend;

        private readonly IBodyProvider<GameEntityId> _bodies;

        private readonly IMotorStateStore _motorState;
        private readonly IActionIntentStore _actions;
        private readonly IAttackChargeStore _charge;

        private readonly IStatsStore _stats;
        private readonly IStatsEffectStore _effects;

        public EntityLifecycle(
            GameState state,
            IEntityIdAllocator ids,
            ISpawnBackend backend,
            IBodyProvider<GameEntityId> bodies,
            IMotorStateStore motorState,
            IActionIntentStore actions,
            IAttackChargeStore charge,
            IStatsStore stats,
            IStatsEffectStore effects)
        {
            _state = state;
            _ids = ids;
            _backend = backend;
            _bodies = bodies;

            _motorState = motorState;
            _actions = actions;
            _charge = charge;

            _stats = stats;
            _effects = effects;
        }

        public GameEntityId Spawn(string prefabKey, float x, float y, GameEntityId? fixedId)
        {
            var id = fixedId ?? _ids.Next();

            // 1) Core entity
            _state.GetOrCreateEntity(id);

            // 2) Unity GO + PhysicsBodyAuthoring.SetEntityId до регистрации
            _backend.Spawn(id, prefabKey, x, y);

            return id;
        }

        public void Despawn(GameEntityId id)
        {
            // 0) Уничтожить GO (PhysicsBodyAuthoring.OnDisable отцепит из registry)
            _backend.Despawn(id);

            // 1) На всякий случай — если GO уже умер/неактивен, это безопасный no-op
            _bodies.Unregister(id);

            // 2) Стереть player->avatar связь если это был аватар
            _state.PlayerAvatars.RemoveByEntity(id);

            // 3) Уборка сторов
            _motorState.Remove(id);
            _actions.Remove(id);
            _charge.Remove(id);
            _effects.ClearEntity(id);
            _stats.Remove(id);

            // 4) Удалить entity из GameState
            _state.RemoveEntity(id);
        }
    }
}