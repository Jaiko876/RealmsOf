using System.Collections.Generic;

namespace Riftborne.Core.Model
{
    public sealed class GameState
    {
        public int Tick { get; private set; }

        private readonly Dictionary<GameEntityId, EntityState> _entities =
            new Dictionary<GameEntityId, EntityState>();

        public IReadOnlyDictionary<GameEntityId, EntityState> Entities => _entities;

        public PlayerAvatarMap PlayerAvatars { get; } = new PlayerAvatarMap();

        public EntityState GetOrCreateEntity(GameEntityId id)
        {
            EntityState state;
            if (!_entities.TryGetValue(id, out state))
            {
                state = new EntityState(id);
                _entities[id] = state;
            }

            return state;
        }

        // convenience: “дай entity, которым управляет игрок”
        public bool TryGetAvatar(PlayerId playerId, out EntityState entity)
        {
            entity = null;

            GameEntityId eid;
            if (!PlayerAvatars.TryGet(playerId, out eid))
                return false;

            return _entities.TryGetValue(eid, out entity);
        }

        public void SetTick(int tick) => Tick = tick;
        public void AdvanceTick() => Tick++;
    }
}
