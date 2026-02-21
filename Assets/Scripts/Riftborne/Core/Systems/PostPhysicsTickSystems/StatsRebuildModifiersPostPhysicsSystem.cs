using System.Collections.Generic;
using Riftborne.Core.Model;
using Riftborne.Core.Stores;
using Riftborne.Core.Stats;

namespace Riftborne.Core.Systems.PostPhysicsTickSystems
{
    public sealed class StatsRebuildModifiersPostPhysicsSystem : IPostPhysicsTickSystem
    {
        private readonly GameState _state;
        private readonly IStatsStore _stats;
        private readonly IStatsEffectStore _effects;

        public StatsRebuildModifiersPostPhysicsSystem(GameState state, IStatsStore stats, IStatsEffectStore effects)
        {
            _state = state;
            _stats = stats;
            _effects = effects;
        }

        public void Tick(int tick)
        {
            foreach (var kv in _state.Entities)
            {
                GameEntityId id = kv.Key;

                if (!_stats.TryGet(id, out var s) || !s.IsInitialized)
                    continue;

                // 1) clear all mods
                ClearAllMods(s);

                // 2) apply effects (deterministic order)
                IReadOnlyList<StatsEffect> list = _effects.GetEffects(id);
                ApplyEffects(s, list);
            }
        }

        private static void ClearAllMods(StatsState s)
        {
            // We can't access StatValue[] directly (private). So:
            // Add a method in StatsState: ClearAllMods()
            // For now assume you add it (see below).
            s.ClearAllMods();
        }

        private static void ApplyEffects(StatsState s, IReadOnlyList<StatsEffect> list)
        {
            int count = list.Count;
            if (count == 0) return;

            // Stable deterministic ordering by Sequence without allocations.
            // Small N, so O(N^2) is fine.
            for (int i = 0; i < count; i++)
            {
                int best = i;
                int bestSeq = list[i].Sequence;

                for (int j = i + 1; j < count; j++)
                {
                    int seq = list[j].Sequence;
                    if (seq < bestSeq)
                    {
                        bestSeq = seq;
                        best = j;
                    }
                }

                StatsEffect e = list[best];
                // we cannot swap in IReadOnlyList; just apply in selected order by scanning
                ApplyOne(s, e);
            }
        }

        private static void ApplyOne(StatsState s, StatsEffect e)
        {
            int stacks = e.Stacks;
            StatMod[] mods = e.Mods;

            for (int i = 0; i < mods.Length; i++)
            {
                var m = mods[i];
                float v = m.Value * stacks;

                if (m.Kind == StatModKind.Add)
                    s.AddFlat(m.Stat, v);
                else
                    s.MulBy(m.Stat, v);
            }
        }
    }
}