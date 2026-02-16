using System;
using System.Collections.Generic;
using Game.Core.Combat.Abilities;
using Game.Core.Combat.Config;
using Game.Core.Combat.Damage;
using Game.Core.Combat.Rules;
using UnityEngine;

namespace Game.Configs
{
    [CreateAssetMenu(menuName = "Game/Config/Combat Config")]
    public sealed class CombatConfigAsset : ScriptableObject
    {
        [Header("Damage")]
        [SerializeField] private DamageTuningData damage = new DamageTuningData();

        [Header("Rules (vanilla)")]
        [SerializeField] private CombatRulesData rules = new CombatRulesData();

        [Header("Abilities")]
        [SerializeField] private List<AbilityEntry> abilities = new List<AbilityEntry>();

        [Header("Hit Query")]
        [SerializeField] private HitQueryData hitQuery = new HitQueryData();

        public DamageTuning ToDamageTuning() => damage.ToCore();

        public CombatRulesConfig ToRulesConfig() => rules.ToCore();

        public HitQueryTuning ToHitQueryTuning() => hitQuery.ToCore();

        public IReadOnlyList<AbilityEntry> Abilities => abilities;

        // ---------- nested ----------
        [Serializable]
        private sealed class DamageTuningData
        {
            [Min(0.0001f)] public float DefenseK = 25f;
            [Range(0f, 1f)] public float MinDamageFraction = 0.05f;
            [Min(0f)] public float MinFlatDamage = 0f;

            public DamageTuning ToCore()
            {
                var t = new DamageTuning();
                t.DefenseK = Mathf.Max(0.0001f, DefenseK);
                t.MinDamageFraction = Mathf.Clamp01(MinDamageFraction);
                t.MinFlatDamage = Mathf.Max(0f, MinFlatDamage);
                return t;
            }
        }

        [Serializable]
        private sealed class CombatRulesData
        {
            [Min(0)] public int DefaultParryWindowTicks = 2;
            [Min(0)] public int DefaultDodgeIFramesTicks = 3;
            [Min(0)] public int DefaultDashIFramesTicks = 0;

            [Min(0)] public int HeavyWindupMinTicks = 6;
            [Min(0)] public int HeavyWindupMaxTicks = 10;

            [Min(0f)] public float ParryStaminaCost = 1f;
            [Min(0f)] public float DodgeStaminaCost = 2f;
            [Min(0f)] public float DashStaminaCost = 1.5f;
            [Min(0f)] public float BlockStaminaCostPerSec = 2f;

            [Min(0f)] public float ParryFailVsHeavy_ExtraStaminaPenalty = 2f;
            [Min(0f)] public float DodgeFailVsLight_ExtraStaggerPenalty = 2.5f;

            [Min(0f)] public float ParrySuccess_StaggerToAttacker = 2f;
            [Min(0f)] public float DodgeSuccess_StaminaDamageToAttacker = 4f;
            [Min(0)] public int DodgeSuccess_MicroStaggerToAttackerTicks = 8;

            public bool AllowParryVsHeavy = false;
            public bool AllowDodgeVsLight = false;

            public bool AttackHitsOncePerAction = true;

            public CombatRulesConfig ToCore()
            {
                var c = new CombatRulesConfig();
                c.DefaultParryWindowTicks = Mathf.Max(0, DefaultParryWindowTicks);
                c.DefaultDodgeIFramesTicks = Mathf.Max(0, DefaultDodgeIFramesTicks);
                c.DefaultDashIFramesTicks = Mathf.Max(0, DefaultDashIFramesTicks);

                c.HeavyWindupMinTicks = Mathf.Max(0, HeavyWindupMinTicks);
                c.HeavyWindupMaxTicks = Mathf.Max(c.HeavyWindupMinTicks, HeavyWindupMaxTicks);

                c.ParryStaminaCost = Mathf.Max(0f, ParryStaminaCost);
                c.DodgeStaminaCost = Mathf.Max(0f, DodgeStaminaCost);
                c.DashStaminaCost = Mathf.Max(0f, DashStaminaCost);
                c.BlockStaminaCostPerSec = Mathf.Max(0f, BlockStaminaCostPerSec);

                c.ParryFailVsHeavy_ExtraStaminaPenalty = Mathf.Max(0f, ParryFailVsHeavy_ExtraStaminaPenalty);
                c.DodgeFailVsLight_ExtraStaggerPenalty = Mathf.Max(0f, DodgeFailVsLight_ExtraStaggerPenalty);

                c.ParrySuccess_StaggerToAttacker = Mathf.Max(0f, ParrySuccess_StaggerToAttacker);
                c.DodgeSuccess_StaminaDamageToAttacker = Mathf.Max(0f, DodgeSuccess_StaminaDamageToAttacker);
                c.DodgeSuccess_MicroStaggerToAttackerTicks = Mathf.Max(0, DodgeSuccess_MicroStaggerToAttackerTicks);

                c.AllowParryVsHeavy = AllowParryVsHeavy;
                c.AllowDodgeVsLight = AllowDodgeVsLight;
                c.AttackHitsOncePerAction = AttackHitsOncePerAction;

                return c;
            }
        }

        [Serializable]
        public sealed class AbilityEntry
        {
            public AbilitySlot Slot;

            [Min(0)] public int WindupTicks;
            [Min(0)] public int ActiveTicks;
            [Min(0)] public int RecoveryTicks;

            [Min(0f)] public float StaminaCost;

            public bool IsAttack;
            public bool IsParry;
            public bool IsDodge;
            public bool IsBlock;

            public bool Parryable;
            public bool Dodgeable;

            [Min(0f)] public float BaseHpDamage;
            [Min(0f)] public float BaseStaminaDamage;
            [Min(0f)] public float BaseStaggerBuild;

            public AbilityDefinition ToCore()
            {
                return new AbilityDefinition(
                    Slot,
                    windupTicks: WindupTicks,
                    activeTicks: ActiveTicks,
                    recoveryTicks: RecoveryTicks,
                    staminaCost: StaminaCost,
                    isAttack: IsAttack,
                    isParry: IsParry,
                    isDodge: IsDodge,
                    isBlock: IsBlock,
                    parryable: Parryable,
                    dodgeable: Dodgeable,
                    baseHpDamage: BaseHpDamage,
                    baseStaminaDamage: BaseStaminaDamage,
                    baseStaggerBuild: BaseStaggerBuild
                );
            }
        }

        [Serializable]
        private sealed class HitQueryData
        {
            [Min(0.01f)] public float HitWidth = 1.0f;
            [Min(0.01f)] public float HitHeight = 0.8f;
            public float ForwardOffset = 0.6f;
            public LayerMask HitMask;

            public HitQueryTuning ToCore()
            {
                return new HitQueryTuning(
                    HitWidth,
                    HitHeight,
                    ForwardOffset,
                    HitMask.value
                );
            }
        }
    }
}
