using System.Collections.Generic;
using Game.Core.Model;

namespace Game.Core.Combat.Health
{
    public sealed class InMemoryHealthStore : IHealthStore
    {
        private readonly Dictionary<GameEntityId, HealthState> _map = new Dictionary<GameEntityId, HealthState>(256);

        public bool Has(GameEntityId entityId)
        {
            return _map.ContainsKey(entityId);
        }

        public HealthState GetOrCreate(GameEntityId entityId, float initialHp)
        {
            HealthState state;
            if (!_map.TryGetValue(entityId, out state))
            {
                state = new HealthState(initialHp, false, lastDamageTick: -1);
                _map[entityId] = state;
            }

            return state;
        }


        public bool TryGet(GameEntityId entityId, out HealthState state)
        {
            return _map.TryGetValue(entityId, out state);
        }

        public void Set(GameEntityId entityId, HealthState state)
        {
            _map[entityId] = state;
        }

        public void Remove(GameEntityId entityId)
        {
            _map.Remove(entityId);
        }
    }
}
