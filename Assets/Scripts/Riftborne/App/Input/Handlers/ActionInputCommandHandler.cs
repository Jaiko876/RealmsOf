using System;
using Riftborne.App.Combat.Abstractions;
using Riftborne.App.Combat.Providers.Abstractions;
using Riftborne.Core.Config;
using Riftborne.Core.Gameplay.Combat.Model;
using Riftborne.Core.Gameplay.Combat.Rules.Abstractions;
using Riftborne.Core.Input;
using Riftborne.Core.Input.Commands;
using Riftborne.Core.Input.Handlers;
using Riftborne.Core.Model;
using Riftborne.Core.Stats;
using Riftborne.Core.Stores.Abstractions;

namespace Riftborne.App.Input.Handlers
{
    public sealed class ActionInputCommandHandler : ICommandHandler<InputCommand>
    {
        private readonly GameState _state;
        private readonly IAttackChargeStore _charge;
        private readonly IAttackHoldStore _hold;
        private readonly IAttackInputRules _rules;
        private readonly ICombatSpeedProvider _speeds;
        private readonly CombatInputTuning _inputTuning;
        private readonly CombatAnimationTuning _animTuning;
        private readonly ICombatActionStarter _starter;

        private readonly IStatsStore _stats;
        private readonly IStatsDeltaStore _deltas;
        private readonly CombatResourceTuning _resources;

        public ActionInputCommandHandler(
            IAttackChargeStore charge,
            IAttackHoldStore hold,
            IAttackInputRules rules,
            ICombatSpeedProvider speeds,
            IGameplayTuning gameplayTuning,
            ICombatActionStarter starter,
            GameState state,
            IStatsStore stats,
            IStatsDeltaStore deltas)
        {
            _charge = charge ?? throw new ArgumentNullException(nameof(charge));
            _hold = hold ?? throw new ArgumentNullException(nameof(hold));
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
            _speeds = speeds ?? throw new ArgumentNullException(nameof(speeds));
            _starter = starter ?? throw new ArgumentNullException(nameof(starter));
            _state = state ?? throw new ArgumentNullException(nameof(state));

            _stats = stats ?? throw new ArgumentNullException(nameof(stats));
            _deltas = deltas ?? throw new ArgumentNullException(nameof(deltas));

            if (gameplayTuning == null) throw new ArgumentNullException(nameof(gameplayTuning));
            _inputTuning = gameplayTuning.CombatInput;
            _animTuning = gameplayTuning.CombatAnimation;
            _resources = gameplayTuning.CombatResources;
        }

        public void Handle(InputCommand command)
        {
            GameEntityId id = command.EntityId;
            int tick = command.Tick;

            bool heldNow = (command.Buttons & InputButtons.AttackHeld) != 0;

            bool prevHeld;
            int heldTicks;
            if (!_hold.TryGet(id, out prevHeld, out heldTicks))
            {
                prevHeld = false;
                heldTicks = 0;
            }

            bool prevCharging = _charge.TryGet(id, out var prevCh, out var _) && prevCh;

            var speeds = _speeds.Get(id);

            var req = new AttackInputStepRequest(
                id, tick, heldNow,
                prevHeld, heldTicks,
                speeds.AttackSpeed, speeds.ChargeSpeed,
                _inputTuning, _animTuning);

            AttackInputStep step = _rules.Step(in req);

            // --- Heavy charge stamina cost (one-shot at charge start) + gating ---
            bool chargingNow = step.Charge.Charging;
            float charge01 = step.Charge.Charge01;

            bool holdOut = step.Hold.IsHeld;
            int heldOut = step.Hold.HeldTicks;

            if (chargingNow && !prevCharging)
            {
                int cost = _resources.HeavyChargeStartStaminaCost;

                if (cost > 0 && !HasStaminaOrAssumeFullOnSpawnTick(id, cost))
                {
                    // Не можем оплатить старт зарядки -> не входим в charging
                    chargingNow = false;
                    charge01 = 0f;

                    // И ключевой момент: не даём "по heldTicks" выпустить heavy бесплатно.
                    int thr = step.Charge.HeavyThresholdTicks;
                    int capped = (thr <= 0) ? 0 : (thr - 1);
                    if (capped < 0) capped = 0;
                    if (heldOut > capped) heldOut = capped;

                    holdOut = true; // всё ещё удерживаем кнопку
                }
                else if (cost > 0)
                {
                    _deltas.SpendStamina(id, cost, StatsDeltaKind.Cost);
                }
            }

            _hold.Set(id, holdOut, heldOut);
            _charge.Set(id, chargingNow, charge01);

            if (!step.Release.HasRelease)
                return;

            if (_state.Entities.TryGetValue(id, out var e) && e != null)
            {
                _starter.TryStartAttack(id, tick, step.Release.Action, step.Release.DurationTicks, step.Release.CooldownTicks, e.Facing);
            }
        }

        private bool HasStaminaOrAssumeFullOnSpawnTick(GameEntityId id, int cost)
        {
            if (cost <= 0) return true;
            if (!_stats.TryGet(id, out var s) || !s.IsInitialized)
                return true;
            return s.StaminaCur >= cost;
        }
    }
}