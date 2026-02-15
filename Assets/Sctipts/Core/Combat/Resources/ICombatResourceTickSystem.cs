using Game.Core.Model;
using Game.Core.Stats;

namespace Game.Core.Combat.Resources
{
    public interface ICombatResourceTickSystem
    {
        void Tick(int tick, float dt);
    }

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
            // В v1 — без полного перебора (как и Health).
            // Позже можно заменить на iterable store.
        }

        // ---- Public helpers для будущих систем ----

        public void SpendStamina(GameEntityId entityId, float amount, int tick)
        {
            if (!_store.TryGetStamina(entityId, out var stamina))
                return;

            stamina.Current -= amount;
            if (stamina.Current < 0f)
                stamina.Current = 0f;

            stamina.LastSpendTick = tick;
            _store.SetStamina(entityId, stamina);
        }

        public void BuildStagger(GameEntityId entityId, float amount, int tick)
        {
            if (!_store.TryGetStagger(entityId, out var stagger))
                return;

            stagger.Current += amount;

            float maxStagger = _stats.Get(entityId, StatId.MaxStagger);
            if (stagger.Current >= maxStagger)
            {
                stagger.Current = maxStagger;
                stagger.IsBroken = true;

                // Vulnerable window = 1 tick сейчас (потом через stat)
                stagger.VulnerableUntilTick = tick + 1;
            }

            _store.SetStagger(entityId, stagger);
        }
    }
}
