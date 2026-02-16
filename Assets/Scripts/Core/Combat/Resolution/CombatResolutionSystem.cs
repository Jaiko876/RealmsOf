using System.Collections.Generic;
using Game.Core.Combat.Abilities;
using Game.Core.Combat.Config;
using Game.Core.Combat.Damage;
using Game.Core.Combat.Resources;
using Game.Core.Combat.Rules;
using Game.Core.Model;
using Game.Core.Stats;

namespace Game.Core.Combat.Resolution
{
    public sealed class CombatResolutionSystem : ICombatResolutionSystem
    {
        private readonly IHitQuery _hitQuery;
        private readonly ICombatActionStore _actions;
        private readonly IHealthDamageService _damage;
        private readonly ICombatResourceStore _resources;
        private readonly ICombatRulesResolver _rules;
        private readonly CombatResourceTuning _resourceTuning;
        private readonly StatResolver _stats;

        private readonly List<GameEntityId> _hits = new List<GameEntityId>(8);
        private readonly HashSet<(AttackAction, GameEntityId)> _alreadyHit
            = new HashSet<(AttackAction, GameEntityId)>();

        public CombatResolutionSystem(
            IHitQuery hitQuery,
            ICombatActionStore actions,
            IHealthDamageService damage,
            ICombatRulesResolver rulesResolver,
            ICombatResourceStore resources,
            CombatResourceTuning resourceTuning,
            StatResolver stats)
        {
            _hitQuery = hitQuery;
            _actions = actions;
            _damage = damage;
            _resources = resources;
            _rules = rulesResolver;
            _resourceTuning = resourceTuning;
            _stats = stats;
        }

        public void ResolveAttack(AttackAction attack, int tick)
        {
            _hits.Clear();
            _hitQuery.QueryHits(attack.Owner, _hits);

            for (int i = 0; i < _hits.Count; i++)
            {
                var target = _hits[i];

                if (_rules.AttackHitsOncePerAction())
                {
                    if (_alreadyHit.Contains((attack, target)))
                        continue;

                    _alreadyHit.Add((attack, target));
                }

                ResolveVsTarget(attack, target, tick);
            }
        }

        private void ResolveVsTarget(AttackAction attack, GameEntityId target, int tick)
        {
            var def = attack.Definition;

            bool hasParry = false;
            bool hasDodge = false;
            bool hasBlock = false;

            var all = _actions.All;
            for (int i = 0; i < all.Count; i++)
            {
                var action = all[i];
                if (!action.Owner.Equals(target))
                    continue;

                if (action is ParryAction parry && parry.IsActive(tick))
                    hasParry = true;

                if (action is DodgeAction dodge && dodge.IsInvulnerable(tick))
                    hasDodge = true;

                if (action is BlockAction block && block.IsActive(tick))
                    hasBlock = true;
            }

            // -----------------------------
            // LIGHT
            // -----------------------------
            if (def.Parryable)
            {
                if (hasParry)
                {
                    float staggerGain = _rules.GetParrySuccessStaggerToAttacker(attack.Owner);
                    BuildStaggerWithBreak(attack.Owner, staggerGain, tick);
                    return;
                }

                if (hasDodge && !_rules.AllowDodgeVsLight(target))
                {
                    float extraStagger = _rules.GetDodgeFailVsLightExtraStaggerPenalty(target);
                    BuildStaggerWithBreak(target, extraStagger, tick);
                }
            }

            // -----------------------------
            // HEAVY
            // -----------------------------
            if (def.Dodgeable)
            {
                if (hasDodge)
                {
                    float staminaPenalty = _rules.GetDodgeSuccessStaminaDamageToAttacker(attack.Owner);
                    AddStamina(attack.Owner, -staminaPenalty);

                    int microTicks = _rules.GetDodgeSuccessMicroStaggerTicksToAttacker(attack.Owner);
                    AddMicroStagger(attack.Owner, microTicks, tick);
                    return;
                }

                if (hasParry && !_rules.AllowParryVsHeavy(target))
                {
                    float staminaPenalty = _rules.GetParryFailVsHeavyExtraStaminaPenalty(target);
                    AddStamina(target, -staminaPenalty);
                }
            }

            // -----------------------------
            // Vulnerable multiplier (broken window)
            // -----------------------------
            float vulnerableMul = 1f;
            if (_resources.TryGetStagger(target, out var st))
            {
                if (st.IsBroken && tick < st.VulnerableUntilTick)
                    vulnerableMul = _resourceTuning.VulnerableHpDamageMultiplier;
            }

            // -----------------------------
            // BLOCK (v1)
            // -----------------------------
            if (hasBlock)
            {
                float blockMul = 0.6f;

                float staminaPenalty = def.BaseHpDamage * 0.75f;
                if (staminaPenalty < 0.25f) staminaPenalty = 0.25f;

                AddStamina(target, -staminaPenalty);

                if (def.Dodgeable) // heavy
                    AddStamina(target, -staminaPenalty);

                bool staminaEmpty = false;
                if (_resources.TryGetStamina(target, out var stm))
                    staminaEmpty = stm.Current <= 0.001f;

                float mul = staminaEmpty ? 0.85f : blockMul;

                var requestBlocked = new DamageRequest(
                    attacker: attack.Owner,
                    target: target,
                    baseHpDamage: def.BaseHpDamage * mul * vulnerableMul,
                    baseStaminaDamage: def.BaseStaminaDamage,
                    baseStaggerBuild: 0f // stagger ведём здесь
                );

                _damage.Apply(requestBlocked, tick);

                BuildStaggerWithBreak(target, def.BaseStaggerBuild, tick);
                return;
            }

            // -----------------------------
            // APPLY DAMAGE
            // -----------------------------
            var request = new DamageRequest(
                attacker: attack.Owner,
                target: target,
                baseHpDamage: def.BaseHpDamage * vulnerableMul,
                baseStaminaDamage: def.BaseStaminaDamage,
                baseStaggerBuild: 0f // stagger ведём здесь
            );

            _damage.Apply(request, tick);

            BuildStaggerWithBreak(target, def.BaseStaggerBuild, tick);
        }

        private void AddStamina(GameEntityId entity, float delta)
        {
            if (_resources.TryGetStamina(entity, out var s))
            {
                s.Current += delta;
                if (s.Current < 0f)
                    s.Current = 0f;

                _resources.SetStamina(entity, s);
            }
        }

        private void BuildStaggerWithBreak(GameEntityId entity, float delta, int tick)
        {
            if (delta <= 0f)
                return;

            if (!_resources.TryGetStagger(entity, out var s))
                return;

            if (s.IsBroken)
                return;

            s.Current += delta;

            float maxStagger = _stats.Get(entity, StatId.MaxStagger);
            if (maxStagger <= 0f) maxStagger = 1f;

            if (s.Current >= maxStagger)
            {
                s.Current = 0f;
                s.IsBroken = true;
                s.VulnerableUntilTick = tick + _resourceTuning.StaggerBrokenWindowTicks;
            }

            _resources.SetStagger(entity, s);
        }

        private void AddMicroStagger(GameEntityId entity, int microTicks, int tick)
        {
            if (microTicks <= 0)
                return;

            if (_resources.TryGetStagger(entity, out var s))
            {
                int until = tick + microTicks;
                if (until > s.VulnerableUntilTick)
                    s.VulnerableUntilTick = until;

                _resources.SetStagger(entity, s);
            }
        }
    }
}
