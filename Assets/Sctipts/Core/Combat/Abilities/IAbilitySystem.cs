using Game.Core.Combat.Resources;
using Game.Core.Model;

namespace Game.Core.Combat.Abilities
{
    public interface IAbilitySystem
    {
        void Use(int tick, GameEntityId entityId, AbilitySlot slot);
    }

    public sealed class AbilitySystem : IAbilitySystem
    {
        private readonly IAbilityDefinitionProvider _definitions;
        private readonly ICombatActionStore _actionStore;
        private readonly ICombatResourceStore _resourceStore;

        public AbilitySystem(
            IAbilityDefinitionProvider definitions,
            ICombatActionStore actionStore,
            ICombatResourceStore resourceStore)
        {
            _definitions = definitions;
            _actionStore = actionStore;
            _resourceStore = resourceStore;
        }

        public void Use(int tick, GameEntityId entityId, AbilitySlot slot)
        {
            var def = _definitions.Get(slot);

            if (_resourceStore.TryGetStamina(entityId, out var stamina))
            {
                if (stamina.Current < def.StaminaCost)
                    return;

                stamina.Current -= def.StaminaCost;
                stamina.LastSpendTick = tick;
                _resourceStore.SetStamina(entityId, stamina);
            }

            if (def.IsAttack)
            {
                var action = new AttackAction(entityId, tick, def);
                _actionStore.Add(action);
            }

            // Parry/Dodge/Block — добавим отдельные Action в следующем шаге
        }
    }
}
