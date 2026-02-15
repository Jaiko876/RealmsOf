using System.Collections.Generic;
using Game.Core.Combat.Abilities;
using Game.Core.Combat.Damage;
using Game.Core.Combat.Resources;
using Game.Core.Combat.Rules;
using Game.Core.Model;

namespace Game.Core.Combat.Resolution
{
    public sealed class CombatResolutionSystem : ICombatResolutionSystem
    {
        private readonly IHitQuery _hitQuery;
        private readonly ICombatActionStore _actions;
        private readonly IHealthDamageService _damage;
        private readonly ICombatResourceStore _resources;
        private readonly ICombatRulesResolver _rules;

        private readonly List<GameEntityId> _hits = new List<GameEntityId>(8);
        private readonly HashSet<(AttackAction, GameEntityId)> _alreadyHit
            = new HashSet<(AttackAction, GameEntityId)>();

        public CombatResolutionSystem(
            IHitQuery hitQuery,
            ICombatActionStore actions,
            IHealthDamageService damage,
            ICombatRulesResolver rulesResolver,
            ICombatResourceStore resources)
        {
            _hitQuery = hitQuery;
            _actions = actions;
            _damage = damage;
            _resources = resources;
            _rules = rulesResolver;
        }

        public void ResolveAttack(AttackAction attack, int tick)
        {
            _hits.Clear();
            _hitQuery.QueryHits(attack.Owner, _hits);

            foreach (var target in _hits)
            {
                if (_rules.AttackHitsOncePerAction())
                {
                    if (_alreadyHit.Contains((attack, target)))
                        continue;

                    _alreadyHit.Add((attack, target));
                }

                ResolveVsTarget(attack, target, tick);
            }
        }

        private void ResolveVsTarget(
            AttackAction attack,
            GameEntityId target,
            int tick)
        {
            var def = attack.Definition;

            bool hasParry = false;
            bool hasDodge = false;

            foreach (var action in _actions.All)
            {
                if (!action.Owner.Equals(target))
                    continue;

                if (action is ParryAction parry &&
                    parry.IsActive(tick))
                {
                    hasParry = true;
                }

                if (action is DodgeAction dodge &&
                    dodge.IsInvulnerable(tick))
                {
                    hasDodge = true;
                }
            }

            // -----------------------------
            // LIGHT
            // -----------------------------
            if (def.Parryable)
            {
                if (hasParry)
                {
                    float staggerGain =
                        _rules.GetParrySuccessStaggerToAttacker(attack.Owner);

                    AddStagger(attack.Owner, staggerGain);
                    return;
                }

                if (hasDodge &&
                    !_rules.AllowDodgeVsLight(target))
                {
                    float extraStagger =
                        _rules.GetDodgeFailVsLightExtraStaggerPenalty(target);

                    AddStagger(target, extraStagger);
                }
            }

            // -----------------------------
            // HEAVY
            // -----------------------------
            if (def.Dodgeable)
            {
                if (hasDodge)
                {
                    float staminaPenalty =
                        _rules.GetDodgeSuccessStaminaDamageToAttacker(attack.Owner);

                    AddStamina(attack.Owner, -staminaPenalty);

                    int microTicks =
                        _rules.GetDodgeSuccessMicroStaggerTicksToAttacker(attack.Owner);

                    AddMicroStagger(attack.Owner, microTicks);

                    return;
                }

                if (hasParry &&
                    !_rules.AllowParryVsHeavy(target))
                {
                    float staminaPenalty =
                        _rules.GetParryFailVsHeavyExtraStaminaPenalty(target);

                    AddStamina(target, -staminaPenalty);
                }
            }

            // -----------------------------
            // APPLY DAMAGE
            // -----------------------------
            var request = new DamageRequest(
                attack.Owner,
                target,
                baseHpDamage: def.BaseHpDamage,
                baseStaminaDamage: def.BaseStaminaDamage,
                baseStaggerBuild: def.BaseStaggerBuild);

            _damage.Apply(request, tick);
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

        private void AddStagger(GameEntityId entity, float delta)
        {
            if (_resources.TryGetStagger(entity, out var s))
            {
                s.Current += delta;
                _resources.SetStagger(entity, s);
            }
        }

        private void AddMicroStagger(GameEntityId entity, int ticks)
        {
            if (_resources.TryGetStagger(entity, out var s))
            {
                s.VulnerableUntilTick += ticks;
                _resources.SetStagger(entity, s);
            }
        }
    }
}
