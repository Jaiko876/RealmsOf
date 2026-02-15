using System;
using System.Collections.Generic;
using Game.Core.Model;

namespace Game.Core.Combat.Rules
{
    public sealed class CombatRulesResolver : ICombatRulesResolver
    {
        private readonly CombatRulesConfig _config;
        private readonly List<ICombatRulesModifier> _mods;

        public CombatRulesResolver(CombatRulesConfig config, IEnumerable<ICombatRulesModifier> modifiers)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            _mods = new List<ICombatRulesModifier>();
            if (modifiers != null)
                _mods.AddRange(modifiers);

            _mods.Sort(CompareByPriority);
        }

        public bool AllowParryVsHeavy(GameEntityId defender)
        {
            float v = _config.AllowParryVsHeavy ? 1f : 0f;
            Apply(defender, CombatRuleId.AllowParryVsHeavy, ref v);
            return v >= 0.5f;
        }

        public bool AllowDodgeVsLight(GameEntityId defender)
        {
            float v = _config.AllowDodgeVsLight ? 1f : 0f;
            Apply(defender, CombatRuleId.AllowDodgeVsLight, ref v);
            return v >= 0.5f;
        }

        public int GetParryWindowTicks(GameEntityId defender)
        {
            float v = _config.DefaultParryWindowTicks;
            Apply(defender, CombatRuleId.ParryWindowTicks, ref v);
            return ClampInt(v, 0, 30);
        }

        public int GetDodgeIFramesTicks(GameEntityId defender)
        {
            float v = _config.DefaultDodgeIFramesTicks;
            Apply(defender, CombatRuleId.DodgeIFramesTicks, ref v);
            return ClampInt(v, 0, 30);
        }

        public int GetDashIFramesTicks(GameEntityId defender)
        {
            float v = _config.DefaultDashIFramesTicks;
            Apply(defender, CombatRuleId.DashIFramesTicks, ref v);
            return ClampInt(v, 0, 30);
        }

        public int GetHeavyWindupMinTicks(GameEntityId attacker)
        {
            float v = _config.HeavyWindupMinTicks;
            Apply(attacker, CombatRuleId.HeavyWindupMinTicks, ref v);
            return ClampInt(v, 0, 300);
        }

        public int GetHeavyWindupMaxTicks(GameEntityId attacker)
        {
            float v = _config.HeavyWindupMaxTicks;
            Apply(attacker, CombatRuleId.HeavyWindupMaxTicks, ref v);
            return ClampInt(v, 0, 300);
        }

        public float GetParryStaminaCost(GameEntityId user)
        {
            float v = _config.ParryStaminaCost;
            Apply(user, CombatRuleId.ParryStaminaCost, ref v);
            return ClampFloat(v, 0f, 999f);
        }

        public float GetDodgeStaminaCost(GameEntityId user)
        {
            float v = _config.DodgeStaminaCost;
            Apply(user, CombatRuleId.DodgeStaminaCost, ref v);
            return ClampFloat(v, 0f, 999f);
        }

        public float GetDashStaminaCost(GameEntityId user)
        {
            float v = _config.DashStaminaCost;
            Apply(user, CombatRuleId.DashStaminaCost, ref v);
            return ClampFloat(v, 0f, 999f);
        }

        public float GetParryFailVsHeavyExtraStaminaPenalty(GameEntityId defender)
        {
            float v = _config.ParryFailVsHeavy_ExtraStaminaPenalty;
            Apply(defender, CombatRuleId.ParryFailVsHeavy_ExtraStaminaPenalty, ref v);
            return ClampFloat(v, 0f, 999f);
        }

        public float GetDodgeFailVsLightExtraStaggerPenalty(GameEntityId defender)
        {
            float v = _config.DodgeFailVsLight_ExtraStaggerPenalty;
            Apply(defender, CombatRuleId.DodgeFailVsLight_ExtraStaggerPenalty, ref v);
            return ClampFloat(v, 0f, 999f);
        }

        public float GetParrySuccessStaggerToAttacker(GameEntityId attacker)
        {
            float v = _config.ParrySuccess_StaggerToAttacker;
            Apply(attacker, CombatRuleId.ParrySuccess_StaggerToAttacker, ref v);
            return ClampFloat(v, 0f, 999f);
        }

        public float GetDodgeSuccessStaminaDamageToAttacker(GameEntityId attacker)
        {
            float v = _config.DodgeSuccess_StaminaDamageToAttacker;
            Apply(attacker, CombatRuleId.DodgeSuccess_StaminaDamageToAttacker, ref v);
            return ClampFloat(v, 0f, 999f);
        }

        public int GetDodgeSuccessMicroStaggerTicksToAttacker(GameEntityId attacker)
        {
            float v = _config.DodgeSuccess_MicroStaggerToAttackerTicks;
            Apply(attacker, CombatRuleId.DodgeSuccess_MicroStaggerToAttackerTicks, ref v);
            return ClampInt(v, 0, 300);
        }

        public bool AttackHitsOncePerAction()
        {
            float v = _config.AttackHitsOncePerAction ? 1f : 0f;
            // глобально — без subject. Передаём default(GameEntityId)
            Apply(default(GameEntityId), CombatRuleId.AttackHitsOncePerAction, ref v);
            return v >= 0.5f;
        }

        private void Apply(GameEntityId subject, CombatRuleId id, ref float value)
        {
            for (int i = 0; i < _mods.Count; i++)
            {
                _mods[i].TryModify(subject, id, ref value);
            }
        }

        private static int CompareByPriority(ICombatRulesModifier a, ICombatRulesModifier b)
        {
            if (a.Priority < b.Priority) return -1;
            if (a.Priority > b.Priority) return 1;
            return 0;
        }

        private static int ClampInt(float v, int min, int max)
        {
            int x = (int)Math.Round(v);
            if (x < min) return min;
            if (x > max) return max;
            return x;
        }

        private static float ClampFloat(float v, float min, float max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }
    }
}
