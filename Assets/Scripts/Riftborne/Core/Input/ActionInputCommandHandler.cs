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
        private readonly IAttackChargeStore _charge;
        private readonly IStatsStore _stats;
        private readonly IAttackCooldownStore _cooldowns;
        private readonly CombatInputTuning _tuning;

        private struct Hold
        {
            public bool PrevHeld;
            public int HeldTicks;
        }

        private readonly Dictionary<GameEntityId, Hold> _hold = new Dictionary<GameEntityId, Hold>();

        public ActionInputCommandHandler(
            IActionIntentStore actions,
            IAttackChargeStore charge,
            IStatsStore stats,
            IAttackCooldownStore cooldowns,
            IGameplayTuning gameplayTuning)
        {
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _charge = charge ?? throw new ArgumentNullException(nameof(charge));
            _stats = stats ?? throw new ArgumentNullException(nameof(stats));
            _cooldowns = cooldowns ?? throw new ArgumentNullException(nameof(cooldowns));

            if (gameplayTuning == null)
                throw new ArgumentNullException(nameof(gameplayTuning));

            _tuning = gameplayTuning.CombatInput;
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
            float chargeSpeed = GetStatClamped(id, StatId.ChargeSpeed, 1f, _tuning.MinChargeSpeed, _tuning.MaxChargeSpeed);

            int heavyThresholdTicks = CeilDiv(_tuning.HeavyThresholdBaseTicks, chargeSpeed);
            int fullChargeExtraTicks = CeilDiv(_tuning.FullChargeExtraBaseTicks, chargeSpeed);

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
                    // Единый кулдаун на "атаку вообще", зависит только от AttackSpeed.
                    // В тюнинге используем LightCooldownBaseTicks как общий параметр (по смыслу это AttackCooldownBaseTicks).
                    int cooldownTicks = ComputeAttackCooldownTicks(id);

                    _cooldowns.ConsumeAttack(id, tick, cooldownTicks);
                    _actions.Set(id, heavy ? ActionState.HeavyAttack : ActionState.LightAttack);
                }

                h.HeldTicks = 0;
                _charge.Set(id, false, 0f);
            }

            h.PrevHeld = held;
            _hold[id] = h;
        }

        private int ComputeAttackCooldownTicks(GameEntityId id)
        {
            float attackSpeed = GetStatClamped(id, StatId.AttackSpeed, 1f, _tuning.MinAttackSpeed, _tuning.MaxAttackSpeed);

            // ВАЖНО: один базовый параметр. Сейчас берём LightCooldownBaseTicks как "общий".
            // Если хочешь — позже переименуем в CombatInputTuning на AttackCooldownBaseTicks и уберём HeavyCooldownBaseTicks из ассета.
            return CeilDiv(_tuning.AttackCooldownBaseTicks, attackSpeed);
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

        // ceil(baseTicks / speed), без Math/MathF (C#9 friendly)
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