using System.Collections.Generic;
using Riftborne.Core.Model;
using Riftborne.Core.Simulation;
using Riftborne.Core.Stores;
using Riftborne.Core.Stats;

namespace Riftborne.Core.Systems.PostPhysicsTickSystems
{
    /// <summary>
    /// Produces regen/decay deltas based on effective stats and tick delta-time.
    /// Does NOT mutate StatsState directly; changes go through IStatsDeltaStore.
    /// Accumulates fractional regen per-entity to be tick-rate independent.
    /// </summary>
    public sealed class StatsRegenPostPhysicsSystem : IPostPhysicsTickSystem
    {
        private struct Accumulator
        {
            public float Hp;
            public float Stamina;
            public float StaggerDecay;
        }

        private readonly GameState _state;
        private readonly IStatsStore _stats;
        private readonly IStatsDeltaStore _deltas;
        private readonly SimulationParameters _sim;

        // Fractional accumulation per entity (so regen works with any tick rate).
        private readonly Dictionary<GameEntityId, Accumulator> _acc = new Dictionary<GameEntityId, Accumulator>();

        public StatsRegenPostPhysicsSystem(
            GameState state,
            IStatsStore stats,
            IStatsDeltaStore deltas,
            SimulationParameters sim)
        {
            _state = state;
            _stats = stats;
            _deltas = deltas;
            _sim = sim;
        }

        public void Tick(int tick)
        {
            float dt = _sim.TickDeltaTime;

            foreach (var kv in _state.Entities)
            {
                var id = kv.Key;

                if (!_stats.TryGet(id, out var s) || !s.IsInitialized)
                    continue;

                _acc.TryGetValue(id, out var a);

                // Accumulate fractional parts. Negative rates are treated as "no regen"
                // (if you want negative regen to be possible, handle it explicitly).
                Accumulate(ref a.Hp, s.GetEffective(StatId.HpRegenPerSec), dt);
                Accumulate(ref a.Stamina, s.GetEffective(StatId.StaminaRegenPerSec), dt);
                Accumulate(ref a.StaggerDecay, s.GetEffective(StatId.StaggerDecayPerSec), dt);

                int hp = ConsumeWhole(ref a.Hp);
                int st = ConsumeWhole(ref a.Stamina);
                int sd = ConsumeWhole(ref a.StaggerDecay);

                // Enqueue deltas (do not mutate state here).
                if (hp != 0) _deltas.Heal(id, hp, StatsDeltaKind.Regen);
                if (st != 0) _deltas.AddStamina(id, st, StatsDeltaKind.Regen);
                if (sd != 0) _deltas.ReduceStagger(id, sd, StatsDeltaKind.Regen);

                _acc[id] = a;
            }

            // Cleanup (optional): If you don't have despawn yet, keep it.
            // Once despawn exists, remove accumulators for missing entities.
        }

        private static void Accumulate(ref float acc, float perSecond, float dt)
        {
            if (perSecond <= 0f || dt <= 0f)
                return;

            acc += perSecond * dt;

            // Guard against float blow-ups if something goes wrong.
            if (acc > 1000000f)
                acc = 1000000f;
        }

        private static int ConsumeWhole(ref float acc)
        {
            if (acc < 1f)
                return 0;

            int whole = (int)acc;
            acc -= whole;

            // Safety: avoid negative due to floating precision.
            if (acc < 0f)
                acc = 0f;

            return whole;
        }
    }
}