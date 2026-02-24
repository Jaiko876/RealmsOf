using System;
using System.Collections.Generic;
using Riftborne.Core.Commands;
using Riftborne.Core.Config;
using Riftborne.Core.Model;
using Riftborne.Core.Model.Animation;
using Riftborne.Core.Stats;
using Riftborne.Core.Stores;

namespace Riftborne.Core.Input
{
    public sealed class ActionInputCommandHandler : ICommandHandler<InputCommand>
    {
        private readonly IActionIntentStore _actions;
        private readonly IActionTimingStore _timings;
        private readonly IAttackChargeStore _charge;
        private readonly IStatsStore _stats;
        private readonly IAttackCooldownStore _cooldowns;
        private readonly CombatInputTuning _inputTuning;
        private readonly CombatAnimationTuning _animTuning;

        private struct Hold
        {
            public bool PrevHeld;
            public int HeldTicks;
        }

        private readonly Dictionary<GameEntityId, Hold> _hold = new Dictionary<GameEntityId, Hold>();

        public ActionInputCommandHandler(
            IActionIntentStore actions,
            IActionTimingStore timings,
            IAttackChargeStore charge,
            IStatsStore stats,
            IAttackCooldownStore cooldowns,
            IGameplayTuning gameplayTuning)
        {
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _timings = timings ?? throw new ArgumentNullException(nameof(timings));
            _charge = charge ?? throw new ArgumentNullException(nameof(charge));
            _stats = stats ?? throw new ArgumentNullException(nameof(stats));
            _cooldowns = cooldowns ?? throw new ArgumentNullException(nameof(cooldowns));
            if (gameplayTuning == null) throw new ArgumentNullException(nameof(gameplayTuning));

            _inputTuning = gameplayTuning.CombatInput;
            _animTuning = gameplayTuning.CombatAnimation;
        }

        public void Handle(InputCommand command)
        {
            var id = command.EntityId;
            var held = (command.Buttons & InputButtons.AttackHeld) != 0;

            _hold.TryGetValue(id, out var h);

            bool heldStarted = held && !h.PrevHeld;
            if (heldStarted)
                h.HeldTicks = 0;

            if (held)
                h.HeldTicks++;

            bool released = (!held && h.PrevHeld);

            // ===== Charge (ChargeSpeed) =====
            float chargeSpeed = GetStatClamped(id, StatId.ChargeSpeed, 1f, _inputTuning.MinChargeSpeed, _inputTuning.MaxChargeSpeed);

            int heavyThresholdTicks = CeilDiv(_inputTuning.HeavyThresholdBaseTicks, chargeSpeed);
            int fullChargeExtraTicks = CeilDiv(_inputTuning.FullChargeExtraBaseTicks, chargeSpeed);

            bool charging = held && (h.HeldTicks >= heavyThresholdTicks);

            float charge01 = 0f;
            if (charging)
            {
                int over = h.HeldTicks - heavyThresholdTicks;
                if (over > 0)
                {
                    float t = over / (float)fullChargeExtraTicks;
                    if (t < 0f) t = 0f;
                    if (t > 1f) t = 1f;
                    charge01 = t;
                }
            }

            _charge.Set(id, charging, charge01);

            // ===== Release -> fire intent with ONE cooldown (AttackSpeed) =====
            if (released)
            {
                bool heavy = h.HeldTicks >= heavyThresholdTicks;
                int tick = command.Tick;

                if (_cooldowns.CanAttack(id, tick))
                {
                    int cooldownTicks = ComputeAttackCooldownTicks(id);

                    _cooldowns.ConsumeAttack(id, tick, cooldownTicks);

                    var action = heavy ? ActionState.HeavyAttack : ActionState.LightAttack;
                    _actions.Set(id, action);

                    // NEW: authoritative "simulation duration" for the triggered action
                    int durationTicks = ComputeAttackActionDurationTicks(id, action);
                    _timings.Set(id, action, durationTicks);
                }

                h.HeldTicks = 0;
                _charge.Set(id, false, 0f);
            }

            h.PrevHeld = held;
            _hold[id] = h;
        }

        private int ComputeAttackCooldownTicks(GameEntityId id)
        {
            float attackSpeed = GetStatClamped(id, StatId.AttackSpeed, 1f, _inputTuning.MinAttackSpeed, _inputTuning.MaxAttackSpeed);
            return CeilDiv(_inputTuning.AttackCooldownBaseTicks, attackSpeed);
        }

        private int ComputeAttackActionDurationTicks(GameEntityId id, ActionState action)
        {
            float attackSpeed = GetStatClamped(id, StatId.AttackSpeed, 1f, _inputTuning.MinAttackSpeed, _inputTuning.MaxAttackSpeed);

            int baseTicks = 0;
            if (action == ActionState.LightAttack) baseTicks = _animTuning.LightAttackDurationBaseTicks;
            else if (action == ActionState.HeavyAttack) baseTicks = _animTuning.HeavyAttackDurationBaseTicks;

            return CeilDiv(baseTicks, attackSpeed);
        }

        private float GetStatClamped(GameEntityId id, StatId stat, float fallback, float min, float max)
        {
            float v = fallback;

            if (_stats.TryGet(id, out var s) && s.IsInitialized)
                v = s.GetEffective(stat);

            if (v < min) return min;
            if (v > max) return max;
            return v;
        }

        private static int CeilDiv(int baseTicks, float speed)
        {
            if (baseTicks <= 0) return 0;
            if (speed <= 0f) return baseTicks;

            float raw = baseTicks / speed;
            int i = (int)raw;
            if (raw > i) i++;

            if (i < 1) i = 1;
            return i;
        }
    }
}