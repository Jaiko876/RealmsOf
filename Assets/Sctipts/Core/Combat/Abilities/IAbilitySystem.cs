using Game.Core.Combat.Resources;
using Game.Core.Model;
using Game.Core.Stats;
using Game.Core.Combat.Rules;

namespace Game.Core.Combat.Abilities
{
    public interface IAbilitySystem
    {
        bool Use(int tick, GameEntityId entityId, AbilitySlot slot);
    }

    public sealed class AbilitySystem : IAbilitySystem
    {
        private readonly IAbilityDefinitionProvider _definitions;
        private readonly ICombatActionStore _actionStore;
        private readonly ICombatResourceStore _resourceStore;
        private readonly StatResolver _stats;
        private readonly ICombatRulesResolver _rules;

        public AbilitySystem(
            IAbilityDefinitionProvider definitions,
            ICombatActionStore actionStore,
            ICombatResourceStore resourceStore,
            StatResolver stats,
            ICombatRulesResolver rules)
        {
            _definitions = definitions;
            _actionStore = actionStore;
            _resourceStore = resourceStore;
            _stats = stats;
            _rules = rules;
        }

        public bool Use(int tick, GameEntityId entityId, AbilitySlot slot)
        {
            var def = _definitions.Get(slot);

            // 1) Блокируем “спам”: пока есть активное действие — новое не стартуем
            var actions = _actionStore.All;
            for (int i = 0; i < actions.Count; i++)
            {
                var a = actions[i];
                if (a.Owner.Equals(entityId) && !a.IsFinished(tick))
                    return false;
            }

            // 2) Ensure stamina exists
            EnsureStamina(entityId);

            // 3) Determine stamina cost (rules for defensive/evade; ability cost for attacks)
            float staminaCost = GetStaminaCost(entityId, slot, def);

            if (!_resourceStore.TryGetStamina(entityId, out var stamina))
                return false;

            if (stamina.Current < staminaCost)
                return false;

            stamina.Current -= staminaCost;
            stamina.LastSpendTick = tick;
            _resourceStore.SetStamina(entityId, stamina);

            // 4) Create action by flags / slot
            if (def.IsAttack)
            {
                _actionStore.Add(new AttackAction(entityId, tick, def));
                return true;
            }

            if (def.IsParry)
            {
                int windowTicks = _rules.GetParryWindowTicks(entityId);
                _actionStore.Add(new ParryAction(entityId, tick, windowTicks));
                return true;
            }

            if (def.IsDodge)
            {
                int iframes = slot == AbilitySlot.Dash
                    ? _rules.GetDashIFramesTicks(entityId)
                    : _rules.GetDodgeIFramesTicks(entityId);

                _actionStore.Add(new DodgeAction(entityId, tick, iframes));
                return true;
            }

            if (def.IsBlock)
            {
                // Блок “потом” по механике — но слот/действие должны существовать, чтобы input работал.
                _actionStore.Add(new ParryAction(entityId, tick, activeTicks: 0));
                return true;
            }

            return false;
        }

        private float GetStaminaCost(GameEntityId user, AbilitySlot slot, AbilityDefinition def)
        {
            if (slot == AbilitySlot.Parry) return _rules.GetParryStaminaCost(user);
            if (slot == AbilitySlot.Dodge) return _rules.GetDodgeStaminaCost(user);
            if (slot == AbilitySlot.Dash) return _rules.GetDashStaminaCost(user);
            // Block cost-per-sec будет отдельной системой (удержание), сейчас не используем.
            return def.StaminaCost;
        }

        private void EnsureStamina(GameEntityId entityId)
        {
            if (_resourceStore.TryGetStamina(entityId, out _))
                return;

            float max = _stats.Get(entityId, StatId.MaxStamina);
            if (max <= 0f) max = 1f;

            _resourceStore.SetStamina(entityId, new StaminaState(max, lastSpendTick: -1));
        }
    }
}
