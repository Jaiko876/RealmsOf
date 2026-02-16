using Game.Core.Combat.Abilities;
using Game.Core.Combat.Config;
using Game.Core.Model;

namespace Game.Core.Combat.Input
{
    public sealed class CombatIntentProcessor
    {
        private readonly CombatInputStateStore _state;
        private readonly CombatInputTuning _tuning;
        private readonly IAbilitySystem _abilities;

        public CombatIntentProcessor(
            CombatInputStateStore state,
            CombatInputTuning tuning,
            IAbilitySystem abilities)
        {
            _state = state;
            _tuning = tuning;
            _abilities = abilities;
        }

        public void PushIntent(int tick, GameEntityId entityId, CombatIntent intent, float dirX)
        {
            if (TryExecute(tick, entityId, intent))
                return;

            var expires = tick + _tuning.InputBufferTicks;
            _state.Enqueue(entityId, new BufferedIntent(entityId, intent, expires, dirX));
        }

        public void Tick(int tick, GameEntityId entityId)
        {
            var buf = _state.GetBuffer(entityId);
            if (buf.Count == 0)
                return;

            for (int i = buf.Count - 1; i >= 0; i--)
            {
                var b = buf[i];

                if (tick > b.ExpiresAtTick)
                {
                    buf.RemoveAt(i);
                    continue;
                }

                if (TryExecute(tick, b.EntityId, b.Intent))
                    buf.RemoveAt(i);
            }
        }

        private bool TryExecute(int tick, GameEntityId entityId, CombatIntent intent)
        {
            switch (intent)
            {
                case CombatIntent.LightAttack: return _abilities.Use(tick, entityId, AbilitySlot.LightAttack);
                case CombatIntent.HeavyAttack: return _abilities.Use(tick, entityId, AbilitySlot.HeavyAttack);
                case CombatIntent.Parry: return _abilities.Use(tick, entityId, AbilitySlot.Parry);
                case CombatIntent.Dodge: return _abilities.Use(tick, entityId, AbilitySlot.Dodge);
                case CombatIntent.Dash: return _abilities.Use(tick, entityId, AbilitySlot.Dash);
                default: return false;
            }
        }
    }
}
