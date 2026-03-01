// Assets/Scripts/Riftborne/Core/Gameplay/Combat/Rules/DefaultCombatRulesEngine.cs
using System;
using Riftborne.Core.Config;
using Riftborne.Core.Gameplay.Combat.Model;
using Riftborne.Core.Gameplay.Combat.Rules.Abstractions;

namespace Riftborne.Core.Gameplay.Combat.Rules
{
    public sealed class DefaultCombatRulesEngine : ICombatRulesEngine
    {
        private readonly CombatDamageTuning _t;

        public DefaultCombatRulesEngine(IGameplayTuning tuning)
        {
            if (tuning == null) throw new ArgumentNullException(nameof(tuning));
            _t = tuning.CombatDamage;
        }

        public CombatHitResult Resolve(in CombatResolutionContext ctx)
        {
            var r = ctx.Request;

            bool isLight = r.Attack == CombatActionType.LightAttack;
            bool isHeavy = r.Attack == CombatActionType.HeavyAttack;
            if (!isLight && !isHeavy)
                return CombatHitResult.None;

            // Auto rules (modifiers)
            bool parry = ctx.EffectiveParry;
            bool dodge = ctx.EffectiveDodge;
            bool block = ctx.EffectiveBlock;

            if (isLight && ctx.Flags.AutoDodgeLight) dodge = true;
            if (isHeavy && ctx.Flags.AutoParryHeavy) parry = true;

            // Allow exceptions
            bool lightDodgeAllowed = ctx.Flags.AllowDodgeLight;
            bool heavyParryAllowed = ctx.Flags.AllowParryHeavy;

            int baseHp = ComputeHpDamage(r.AttackerAttack, r.DefenderDefense);

            // --- BLOCK has priority over "wrong reaction" (defender is holding block) ---
            if (block)
            {
                if (isLight)
                {
                    return new CombatHitResult(
                        defenderHpDamage: 0,
                        defenderStaminaDamage: _t.BlockLightStaminaDamage,
                        defenderStaggerBuild: _t.BlockLightStaggerBuild,
                        attackerStaminaDamage: 0,
                        attackerStaggerBuild: 0);
                }

                // heavy vs block: strong
                int hp = (int)MathF.Round(baseHp * _t.HeavyHpMul * _t.BlockHeavyHpMul);
                return new CombatHitResult(
                    defenderHpDamage: hp,
                    defenderStaminaDamage: _t.BlockHeavyStaminaDamage,
                    defenderStaggerBuild: _t.BlockHeavyStaggerBuild,
                    attackerStaminaDamage: 0,
                    attackerStaggerBuild: 0);
            }

            // --- SUCCESS reactions ---
            // Light can be parried; Heavy can be dodged.
            // Exceptions allow: parry heavy / dodge light.
            if (isLight)
            {
                if (parry)
                {
                    return new CombatHitResult(0, 0, 0, 0, _t.ParrySuccessAttackerStagger);
                }

                if (dodge && lightDodgeAllowed)
                {
                    return new CombatHitResult(0, 0, 0, _t.DodgeSuccessAttackerStaminaDamage, _t.DodgeSuccessAttackerStagger);
                }
            }
            else // heavy
            {
                if (dodge)
                {
                    return new CombatHitResult(0, 0, 0, _t.DodgeSuccessAttackerStaminaDamage, _t.DodgeSuccessAttackerStagger);
                }

                if (parry && heavyParryAllowed)
                {
                    return new CombatHitResult(0, 0, 0, 0, _t.ParrySuccessAttackerStagger);
                }
            }

            // --- FAIL reactions (wrong choice) ---
            if (isHeavy && parry && !heavyParryAllowed)
            {
                int hp = (int)MathF.Round(baseHp * _t.HeavyHpMul);
                return new CombatHitResult(hp, _t.ParryFailDefenderStaminaDamage, _t.HeavyStagger, 0, 0);
            }

            if (isLight && dodge && !lightDodgeAllowed)
            {
                int hp = (int)MathF.Round(baseHp * _t.LightHpMul);
                return new CombatHitResult(hp, _t.LightStaminaDamage, _t.LightStagger + _t.DodgeFailExtraDefenderStagger, 0, 0);
            }

            // --- Normal hit ---
            if (isLight)
            {
                int hp = (int)MathF.Round(baseHp * _t.LightHpMul);
                return new CombatHitResult(hp, _t.LightStaminaDamage, _t.LightStagger, 0, 0);
            }
            else
            {
                int hp = (int)MathF.Round(baseHp * _t.HeavyHpMul);
                return new CombatHitResult(hp, _t.HeavyStaminaDamage, _t.HeavyStagger, 0, 0);
            }
        }

        private static int ComputeHpDamage(float attackerAttack, float defenderDefense)
        {
            float v = attackerAttack - defenderDefense;
            if (v < 1f) v = 1f;
            return (int)MathF.Round(v);
        }
    }
}