using System;
using Riftborne.Core.Config;
using Riftborne.Core.Gameplay.Combat.Model;
using Riftborne.Core.Gameplay.Combat.Rules.Abstractions;

namespace Riftborne.Core.Gameplay.Combat.Rules
{
    public sealed class BasicCombatHitRules : ICombatHitRules
    {
        private readonly CombatDamageTuning _t;

        public BasicCombatHitRules(IGameplayTuning tuning)
        {
            if (tuning == null) throw new ArgumentNullException(nameof(tuning));
            _t = tuning.CombatDamage;
        }

        public CombatHitResult Resolve(in CombatHitContext ctx)
        {
            // Damage base from stats
            int baseHp = ComputeHpDamage(ctx.AttackerAttack, ctx.DefenderDefense);

            bool isLight = ctx.Attack == CombatActionType.LightAttack;
            bool isHeavy = ctx.Attack == CombatActionType.HeavyAttack;

            if (!isLight && !isHeavy)
                return CombatHitResult.None;

            // --- Defender reactions (v1 rules) ---
            // Light can be parried; Heavy can be dodged.
            if (isLight && ctx.DefenderParryActive)
            {
                // Parry success: negate damage, punish attacker (stagger only, per твоему канону)
                return new CombatHitResult(
                    defenderHpDamage: 0,
                    defenderStaminaDamage: 0,
                    defenderStaggerBuild: 0,
                    attackerStaminaDamage: 0,
                    attackerStaggerBuild: _t.ParrySuccessAttackerStagger);
            }

            if (isHeavy && ctx.DefenderDodgeActive)
            {
                // Dodge success: negate damage, punish attacker hard on stamina + micro stagger
                return new CombatHitResult(
                    defenderHpDamage: 0,
                    defenderStaminaDamage: 0,
                    defenderStaggerBuild: 0,
                    attackerStaminaDamage: _t.DodgeSuccessAttackerStaminaDamage,
                    attackerStaggerBuild: _t.DodgeSuccessAttackerStagger);
            }

            // Fail cases:
            if (isHeavy && ctx.DefenderParryActive)
            {
                // Tried to parry heavy => full heavy + defender stamina damage
                int hp = (int)MathF.Round(baseHp * _t.HeavyHpMul);
                return new CombatHitResult(
                    defenderHpDamage: hp,
                    defenderStaminaDamage: _t.ParryFailDefenderStaminaDamage,
                    defenderStaggerBuild: _t.HeavyStagger,
                    attackerStaminaDamage: 0,
                    attackerStaggerBuild: 0);
            }

            if (isLight && ctx.DefenderDodgeActive)
            {
                // Tried to dodge light => full light + extra stagger
                int hp = (int)MathF.Round(baseHp * _t.LightHpMul);
                return new CombatHitResult(
                    defenderHpDamage: hp,
                    defenderStaminaDamage: _t.LightStaminaDamage,
                    defenderStaggerBuild: _t.LightStagger + _t.DodgeFailExtraDefenderStagger,
                    attackerStaminaDamage: 0,
                    attackerStaggerBuild: 0);
            }

            // No defense => normal hit
            if (isLight)
            {
                int hp = (int)MathF.Round(baseHp * _t.LightHpMul);
                return new CombatHitResult(hp, _t.LightStaminaDamage, _t.LightStagger, 0, 0);
            }

            // heavy
            {
                int hp = (int)MathF.Round(baseHp * _t.HeavyHpMul);
                return new CombatHitResult(hp, _t.HeavyStaminaDamage, _t.HeavyStagger, 0, 0);
            }
        }

        private static int ComputeHpDamage(float attackerAttack, float defenderDefense)
        {
            // Very simple v1: max(1, attack - defense)
            float v = attackerAttack - defenderDefense;
            if (v < 1f) v = 1f;
            return (int)MathF.Round(v);
        }
    }
}