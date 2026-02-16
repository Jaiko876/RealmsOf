using System.Collections.Generic;

namespace Game.Core.Combat.Abilities
{
    public interface IAbilityDefinitionProvider
    {
        AbilityDefinition Get(AbilitySlot slot);
    }

    public sealed class DefaultAbilityDefinitionProvider : IAbilityDefinitionProvider
    {
        private readonly Dictionary<AbilitySlot, AbilityDefinition> _map;

        public DefaultAbilityDefinitionProvider()
        {
            _map = new Dictionary<AbilitySlot, AbilityDefinition>
            {
                {
                    AbilitySlot.LightAttack,
                    new AbilityDefinition(
                        AbilitySlot.LightAttack,
                        windupTicks: 3,
                        activeTicks: 2,
                        recoveryTicks: 5,
                        staminaCost: 1f,
                        isAttack: true,
                        isParry: false,
                        isDodge: false,
                        isBlock: false,
                        parryable: true,
                        dodgeable: false,
                        baseHpDamage: 2f,
                        baseStaminaDamage: 1f,
                        baseStaggerBuild: 2f)
                },
                {
                    AbilitySlot.HeavyAttack,
                    new AbilityDefinition(
                        AbilitySlot.HeavyAttack,
                        windupTicks: 6,
                        activeTicks: 2,
                        recoveryTicks: 8,
                        staminaCost: 3f,
                        isAttack: true,
                        isParry: false,
                        isDodge: false,
                        isBlock: false,
                        parryable: false,
                        dodgeable: true,
                        baseHpDamage: 4f,
                        baseStaminaDamage: 0f,
                        baseStaggerBuild: 4f)
                },
                {
                    AbilitySlot.Parry,
                    new AbilityDefinition(
                        AbilitySlot.Parry,
                        windupTicks: 0,
                        activeTicks: 2,
                        recoveryTicks: 4,
                        staminaCost: 1f,
                        isAttack: false,
                        isParry: true,
                        isDodge: false,
                        isBlock: false,
                        parryable: false,
                        dodgeable: false,
                        baseHpDamage: 0f,
                        baseStaminaDamage: 0f,
                        baseStaggerBuild: 0f)
                },
                {
                    AbilitySlot.Dodge,
                    new AbilityDefinition(
                        AbilitySlot.Dodge,
                        windupTicks: 0,
                        activeTicks: 3,
                        recoveryTicks: 6,
                        staminaCost: 2f,
                        isAttack: false,
                        isParry: false,
                        isDodge: true,
                        isBlock: false,
                        parryable: false,
                        dodgeable: false,
                        baseHpDamage: 0f,
                        baseStaminaDamage: 0f,
                        baseStaggerBuild: 0f)
                },
                {
                    AbilitySlot.Dash,
                    new AbilityDefinition(
                        AbilitySlot.Dash,
                        windupTicks: 0,
                        activeTicks: 0, // i-frames берём из rules
                        recoveryTicks: 0,
                        staminaCost: 1.5f,
                        isAttack: false,
                        isParry: false,
                        isDodge: true,
                        isBlock: false,
                        parryable: false,
                        dodgeable: false,
                        baseHpDamage: 0f,
                        baseStaminaDamage: 0f,
                        baseStaggerBuild: 0f)
                },
                {
                    AbilitySlot.Block,
                    new AbilityDefinition(
                        AbilitySlot.Block,
                        windupTicks: 0,
                        activeTicks: 0,
                        recoveryTicks: 0,
                        staminaCost: 0f,
                        isAttack: false,
                        isParry: false,
                        isDodge: false,
                        isBlock: true,
                        parryable: false,
                        dodgeable: false,
                        baseHpDamage: 0f,
                        baseStaminaDamage: 0f,
                        baseStaggerBuild: 0f)
                }
            };
        }

        public AbilityDefinition Get(AbilitySlot slot)
        {
            return _map[slot];
        }
    }
}
