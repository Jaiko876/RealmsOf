using System.Collections.Generic;
using Riftborne.Core.Gameplay.Resources;
using Riftborne.Core.Model;
using Riftborne.Core.Stats;
using Riftborne.Core.Stores.Abstractions;
using Riftborne.Core.TIme;

namespace Riftborne.Core.Systems.PostPhysicsTickSystems
{
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
        private readonly IResourceRegenPolicy _policy;

        private readonly Dictionary<GameEntityId, Accumulator> _acc = new Dictionary<GameEntityId, Accumulator>();
        private readonly List<GameEntityId> _tmpKeys = new List<GameEntityId>(64);

        public StatsRegenPostPhysicsSystem(
            GameState state,
            IStatsStore stats,
            IStatsDeltaStore deltas,
            SimulationParameters sim)
            : this(state, stats, deltas, sim, null)
        {
        }

        public StatsRegenPostPhysicsSystem(
            GameState state,
            IStatsStore stats,
            IStatsDeltaStore deltas,
            SimulationParameters sim,
            IResourceRegenPolicy policy)
        {
            _state = state;
            _stats = stats;
            _deltas = deltas;
            _sim = sim;
            _policy = policy;
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

                bool canStamina = _policy == null || _policy.CanRegen(id, StatsResource.Stamina, tick);

                Accumulate(ref a.Hp, s.GetEffective(StatId.HpRegenPerSec), dt);
                if (canStamina)
                    Accumulate(ref a.Stamina, s.GetEffective(StatId.StaminaRegenPerSec), dt);
                Accumulate(ref a.StaggerDecay, s.GetEffective(StatId.StaggerDecayPerSec), dt);

                int hp = ConsumeWhole(ref a.Hp);
                int st = canStamina ? ConsumeWhole(ref a.Stamina) : 0;
                int sd = ConsumeWhole(ref a.StaggerDecay);

                if (hp != 0) _deltas.Heal(id, hp, StatsDeltaKind.Regen);
                if (st != 0) _deltas.AddStamina(id, st, StatsDeltaKind.Regen);
                if (sd != 0) _deltas.ReduceStagger(id, sd, StatsDeltaKind.Regen);

                _acc[id] = a;
            }

            if (_acc.Count > 0)
            {
                _tmpKeys.Clear();
                foreach (var k in _acc.Keys) _tmpKeys.Add(k);
                for (int i = 0; i < _tmpKeys.Count; i++)
                {
                    var id = _tmpKeys[i];
                    if (!_state.Entities.ContainsKey(id))
                        _acc.Remove(id);
                }
            }
        }

        private static void Accumulate(ref float acc, float perSecond, float dt)
        {
            if (perSecond <= 0f || dt <= 0f)
                return;
            acc += perSecond * dt;
            if (acc > 1_000_000f) acc = 1_000_000f;
        }

        private static int ConsumeWhole(ref float acc)
        {
            int whole = (int)acc;
            if (whole <= 0) return 0;
            acc -= whole;
            if (acc < 0f) acc = 0f;
            return whole;
        }
    }
}