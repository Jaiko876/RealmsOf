using System.Collections.Generic;
using Riftborne.Core.Commands;
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

        // =========================
        // Heavy charge (ChargeSpeed)
        // =========================

        // Сколько тиков надо держать, чтобы включить charge (при ChargeSpeed=1).
        // При TickRate=50: 18 тиков ≈ 0.36s
        private const int HeavyThresholdBaseTicks = 18;

        // За сколько тиков после порога набираем Charge01 до 1 (при ChargeSpeed=1).
        // При TickRate=50: 42 тика ≈ 0.84s
        private const int FullChargeExtraBaseTicks = 42;

        // =========================
        // Attack cooldown (AttackSpeed)
        // =========================

        // Базовые кд (при AttackSpeed=1).
        // Это ограничивает частоту появления intent-ов атак.
        private const int LightCooldownBaseTicks = 18; // ~0.36s @ 50 tps
        private const int HeavyCooldownBaseTicks = 26; // ~0.52s @ 50 tps

        // Guard rails
        private const float MinAttackSpeed = 0.20f;
        private const float MaxAttackSpeed = 3.00f;

        private const float MinChargeSpeed = 0.20f;
        private const float MaxChargeSpeed = 3.00f;

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
            IAttackCooldownStore cooldowns)
        {
            _actions = actions;
            _charge = charge;
            _stats = stats;
            _cooldowns = cooldowns;
        }

        public void Handle(InputCommand command)
        {
            var id = command.EntityId;

            var held = (command.Buttons & InputButtons.AttackHeld) != 0;

            _hold.TryGetValue(id, out var h);

            bool heldStarted = held && !h.PrevHeld;
            if (heldStarted)
                h.HeldTicks = 0;

            // тик удержания
            if (held)
                h.HeldTicks++;

            // отпускание (release edge)
            bool released = (!held && h.PrevHeld);

            // вычислим масштабированные тики heavy threshold и full charge от ChargeSpeed
            int heavyThresholdTicks = HeavyThresholdBaseTicks;
            int fullChargeExtraTicks = FullChargeExtraBaseTicks;

            float chargeSpeed = GetEffectiveStatOrDefault(id, StatId.ChargeSpeed, 1f);
            chargeSpeed = Clamp(chargeSpeed, MinChargeSpeed, MaxChargeSpeed);

            heavyThresholdTicks = CeilDiv(HeavyThresholdBaseTicks, chargeSpeed);
            fullChargeExtraTicks = CeilDiv(FullChargeExtraBaseTicks, chargeSpeed);

            // вычислим charging + charge01
            bool charging = held && (h.HeldTicks >= heavyThresholdTicks);
            float charge01 = 0f;

            if (charging)
            {
                int over = h.HeldTicks - heavyThresholdTicks;
                if (over <= 0)
                {
                    charge01 = 0f;
                }
                else
                {
                    float t = over / (float)fullChargeExtraTicks;
                    if (t < 0f) t = 0f;
                    if (t > 1f) t = 1f;
                    charge01 = t;
                }
            }

            // пишем store (персистентный визуальный флаг)
            _charge.Set(id, charging, charge01);

            // на отпускании решаем, что было: light или heavy
            // + гейт по кд, зависящему от AttackSpeed
            if (released)
            {
                bool heavy = h.HeldTicks >= heavyThresholdTicks;

                int tick = command.Tick;

                if (_cooldowns.CanAttack(id, tick))
                {
                    int cooldownTicks = ComputeCooldownTicks(id, heavy);
                    _cooldowns.ConsumeAttack(id, tick, cooldownTicks);

                    _actions.Set(id, heavy ? ActionState.HeavyAttack : ActionState.LightAttack);
                }

                // сброс (даже если кд не позволил)
                h.HeldTicks = 0;
                _charge.Set(id, false, 0f);
            }

            h.PrevHeld = held;
            _hold[id] = h;
        }

        private int ComputeCooldownTicks(GameEntityId id, bool heavy)
        {
            float attackSpeed = GetEffectiveStatOrDefault(id, StatId.AttackSpeed, 1f);
            attackSpeed = Clamp(attackSpeed, MinAttackSpeed, MaxAttackSpeed);

            int baseTicks = heavy ? HeavyCooldownBaseTicks : LightCooldownBaseTicks;
            return CeilDiv(baseTicks, attackSpeed);
        }

        private float GetEffectiveStatOrDefault(GameEntityId id, StatId stat, float fallback)
        {
            if (_stats == null)
                return fallback;

            if (!_stats.TryGet(id, out var s) || !s.IsInitialized)
                return fallback;

            return s.GetEffective(stat);
        }

        private static float Clamp(float v, float min, float max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }

        // ceil(baseTicks / speed), но без Math/MathF
        private static int CeilDiv(int baseTicks, float speed)
        {
            if (baseTicks <= 0) return 0;
            if (speed <= 0f) return baseTicks;

            float raw = baseTicks / speed;
            int i = (int)raw;
            if (raw > i) i++;

            // чтобы не получить 0 тиков при огромной speed
            if (i < 1) i = 1;

            return i;
        }
    }
}