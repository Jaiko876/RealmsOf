using System.Collections.Generic;
using Riftborne.Core.Stores;
using Riftborne.Core.Stats;

namespace Riftborne.Core.Systems.PostPhysicsTickSystems
{
    public sealed class StatsApplyDeltasPostPhysicsSystem : IPostPhysicsTickSystem
    {
        private readonly IStatsStore _stats;
        private readonly IStatsDeltaStore _deltas;

        public StatsApplyDeltasPostPhysicsSystem(IStatsStore stats, IStatsDeltaStore deltas)
        {
            _stats = stats;
            _deltas = deltas;
        }

        public void Tick(int tick)
        {
            IReadOnlyList<StatsDelta> list = _deltas.Drain();

            for (int i = 0; i < list.Count; i++)
            {
                var d = list[i];

                if (!_stats.TryGet(d.Target, out var s) || !s.IsInitialized)
                    continue;

                Apply(s, d);
            }

            _deltas.Clear();
        }

        private static void Apply(StatsState s, StatsDelta d)
        {
            switch (d.Resource)
            {
                case StatsResource.Hp:
                    s.AddHp(d.Amount);
                    break;

                case StatsResource.Stamina:
                    s.AddStamina(d.Amount);
                    break;

                case StatsResource.Stagger:
                    s.AddStagger(d.Amount);
                    break;
            }
        }
    }
}