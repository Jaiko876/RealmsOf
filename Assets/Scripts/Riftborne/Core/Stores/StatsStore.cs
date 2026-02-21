using System.Collections.Generic;
using Riftborne.Core.Model;
using Riftborne.Core.Stats;

namespace Riftborne.Core.Stores
{
    public sealed class StatsStore : IStatsStore
    {
        private readonly Dictionary<GameEntityId, StatsState> _map = new Dictionary<GameEntityId, StatsState>();

        public StatsState GetOrCreate(GameEntityId id)
        {
            if (_map.TryGetValue(id, out var s))
                return s;

            s = new StatsState(id);
            _map[id] = s;
            return s;
        }

        public bool TryGet(GameEntityId id, out StatsState state) => _map.TryGetValue(id, out state);

        public void Remove(GameEntityId id) => _map.Remove(id);

        public void Clear() => _map.Clear();
    }
}