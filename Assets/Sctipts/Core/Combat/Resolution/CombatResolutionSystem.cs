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
            bool hasBlock = false;


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
                if (action is BlockAction block && block.IsActive(tick))
                {
                    hasBlock = true;
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
            // BLOCK (v1)
            // -----------------------------
            // Если парри активен и удар парируемый — парри уже обработал и вернул.
            // Если додж успешен для heavy — мы уже вернули.
            // Здесь блок работает как "смягчение" оставшихся случаев.
            if (hasBlock)
            {
                // Срежем HP-урон. Коэффициент возьмём из статов, дефолт уже есть.
                // Если у тебя позже будет броня/перки — они просто поменяют этот стат.
                float blockMul = 0.6f; // fallback
                                       // StatResolver тут нет — поэтому используем правила через ресурсы/конфиг в v1.
                                       // (Если хочешь — я в следующем шаге протяну StatResolver в CombatResolutionSystem.)
                                       // Пока сделаем проще: blockMul = 0.6.

                // За блок платим стаминой: чем сильнее удар, тем больнее.
                // В v1: базируемся на BaseHpDamage (потом переведём на staminaDamage/impact).
                float staminaPenalty = def.BaseHpDamage * 0.75f;
                if (staminaPenalty < 0.25f) staminaPenalty = 0.25f;

                AddStamina(target, -staminaPenalty);

                // Heavy сильнее пробивает блок: добавим ещё штрафа.
                if (def.Dodgeable) // heavy (у тебя heavy = dodgeable)
                {
                    AddStamina(target, -staminaPenalty);
                }

                // И режем урон (сейчас просто уменьшим base перед Apply)
                // Если стамина упала в 0 — блок считается "продавленным", урон режем слабее.
                bool staminaEmpty = false;
                if (_resources.TryGetStamina(target, out var st))
                    staminaEmpty = st.Current <= 0.001f;

                float mul = staminaEmpty ? 0.85f : blockMul;

                var requestBlocked = new DamageRequest(
                    attack.Owner,
                    target,
                    baseHpDamage: def.BaseHpDamage * mul,
                    baseStaminaDamage: def.BaseStaminaDamage,
                    baseStaggerBuild: def.BaseStaggerBuild);

                _damage.Apply(requestBlocked, tick);
                return;
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
