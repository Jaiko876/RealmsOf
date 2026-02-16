using Game.Core.Model;
using Game.Core.Stats;

namespace Game.Core.Combat.Resources
{
    public interface ICombatResourceTickSystem
    {
        void Tick(int tick, float dt);
    }

    /// <summary>
    /// V1: без полного перебора всех entity (как и HealthTickSystem).
    /// Здесь лежат helpers: spend/regen/build/decay для тех, кто их вызывает.
    /// Когда появится iterable entity list (например GameState.Entities),
    /// можно сделать реген/декей в Tick().
    /// </summary>
    public sealed class CombatResourceTickSystem : ICombatResourceTickSystem
    {
        private readonly ICombatResourceStore _store;
        private readonly StatResolver _stats;

        public CombatResourceTickSystem(ICombatResourceStore store, StatResolver stats)
        {
            _store = store;
            _stats = stats;
        }

        public void Tick(int tick, float dt)
        {
            // В v1 — пусто (нет перебора сущностей).
            // Реген/декей делаем по месту через публичные методы ниже.
        }

        // --------------------
        // Stamina
        // --------------------

        public void SpendStamina(GameEntityId entityId, float amount, int tick)
        {
            if (amount <= 0f)
                return;

            if (!_store.TryGetStamina(entityId, out var stamina))
                return;

            stamina.Current -= amount;
            if (stamina.Current < 0f)
                stamina.Current = 0f;

            stamina.LastSpendTick = tick;
            _store.SetStamina(entityId, stamina);
        }

        public void RegenStamina(GameEntityId entityId, float regenPerSec, int regenDelayTicks, int tick, float dt)
        {
            if (regenPerSec <= 0f)
                return;

            if (!_store.TryGetStamina(entityId, out var stamina))
                return;

            if (stamina.LastSpendTick >= 0 && tick - stamina.LastSpendTick < regenDelayTicks)
                return;

            float max = _stats.Get(entityId, StatId.MaxStamina);
            if (max <= 0f) max = 1f;

            stamina.Current += regenPerSec * dt;
            if (stamina.Current > max)
                stamina.Current = max;

            _store.SetStamina(entityId, stamina);
        }

        // --------------------
        // Stagger
        // --------------------

        public void BuildStagger(GameEntityId entityId, float amount, int tick, int brokenWindowTicks)
        {
            if (amount <= 0f)
                return;

            if (!_store.TryGetStagger(entityId, out var stagger))
                return;

            if (stagger.IsBroken)
                return;

            stagger.Current += amount;

            float maxStagger = _stats.Get(entityId, StatId.MaxStagger);
            if (maxStagger <= 0f) maxStagger = 1f;

            if (stagger.Current >= maxStagger)
            {
                stagger.Current = 0f;
                stagger.IsBroken = true;
                stagger.VulnerableUntilTick = tick + brokenWindowTicks;
            }

            _store.SetStagger(entityId, stagger);
        }

        public void DecayStagger(GameEntityId entityId, float decayPerSec, int tick, float dt)
        {
            if (decayPerSec <= 0f)
                return;

            if (!_store.TryGetStagger(entityId, out var stagger))
                return;

            if (stagger.IsBroken)
                return;

            stagger.Current -= decayPerSec * dt;
            if (stagger.Current < 0f)
                stagger.Current = 0f;

            _store.SetStagger(entityId, stagger);
        }

        public void TickBrokenWindow(GameEntityId entityId, int tick)
        {
            if (!_store.TryGetStagger(entityId, out var stagger))
                return;

            if (!stagger.IsBroken)
                return;

            if (tick >= stagger.VulnerableUntilTick)
            {
                stagger.IsBroken = false;
                stagger.Current = 0f;
                _store.SetStagger(entityId, stagger);
            }
        }

        public void AddMicroStagger(GameEntityId entityId, int microTicks, int tick)
        {
            if (microTicks <= 0)
                return;

            if (!_store.TryGetStagger(entityId, out var stagger))
                return;

            int until = tick + microTicks;
            if (until > stagger.VulnerableUntilTick)
                stagger.VulnerableUntilTick = until;

            _store.SetStagger(entityId, stagger);
        }
    }
}
