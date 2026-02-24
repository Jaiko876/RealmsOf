using Riftborne.Core.Model;
using Riftborne.Core.Stats;
using Riftborne.Core.Stores.Abstractions;

namespace Riftborne.Core.Systems.PostPhysicsTickSystems
{
    public sealed class StatsInitPostPhysicsSystem : IPostPhysicsTickSystem
    {
        private readonly GameState _state;
        private readonly IStatsStore _stats;
        private readonly StatsDefaults _defaults;

        public StatsInitPostPhysicsSystem(GameState state, IStatsStore stats, StatsDefaults defaults)
        {
            _state = state;
            _stats = stats;
            _defaults = defaults;
        }

        public void Tick(int tick)
        {
            foreach (var kv in _state.Entities)
            {
                var id = kv.Key;
                var s = _stats.GetOrCreate(id);

                if (!s.IsInitialized)
                    s.InitializeFromDefaults(_defaults);
            }
        }
    }
}