using Riftborne.Core.Config;
using Riftborne.Core.Gameplay.Combat.Model;
using Riftborne.Core.Gameplay.Combat.Rules.Abstractions;
using Riftborne.Core.Model.Animation;

namespace Riftborne.Core.Gameplay.Combat.Rules
{
    public sealed class AttackInputRules : IAttackInputRules
    {
        public AttackInputStep Step(in AttackInputStepRequest r)
        {
            HoldState hold = UpdateHold(r.PrevHeld, r.HeldNow, r.HeldTicks);

            ChargeState charge = ComputeCharge(
                hold.IsHeld,
                hold.HeldTicks,
                r.ChargeSpeed,
                r.InputTuning);

            ReleaseDecision release = ReleaseDecision.None();

            if (hold.ReleasedThisTick)
            {
                bool heavy = hold.HeldTicks >= charge.HeavyThresholdTicks;

                ActionState action = heavy ? ActionState.HeavyAttack : ActionState.LightAttack;

                int cooldownTicks = CeilDiv(r.InputTuning.AttackCooldownBaseTicks, r.AttackSpeed);

                int baseDuration = action == ActionState.HeavyAttack
                    ? r.AnimTuning.HeavyAttackDurationBaseTicks
                    : r.AnimTuning.LightAttackDurationBaseTicks;

                int durationTicks = CeilDiv(baseDuration, r.AttackSpeed);

                release = new ReleaseDecision(true, action, durationTicks, cooldownTicks);
                
                hold = new HoldState(false, 0, true);
                charge = new ChargeState(false, 0f, charge.HeavyThresholdTicks);
            }

            return new AttackInputStep(hold, charge, release);
        }

        private static HoldState UpdateHold(bool prevHeld, bool heldNow, int heldTicks)
        {
            // ВАЖНО: тут ты можешь зафиксировать “семантику heldTicks”
            // например: heldTicks = число тиков с момента нажатия (0 на первом тике)
            bool started = heldNow && !prevHeld;
            if (started)
                heldTicks = 0;

            if (heldNow)
                heldTicks++;

            bool released = !heldNow && prevHeld;

            return new HoldState(heldNow, heldTicks, released);
        }

        private static ChargeState ComputeCharge(bool heldNow, int heldTicks, float chargeSpeed,
            CombatInputTuning tuning)
        {
            int heavyThresholdTicks = CeilDiv(tuning.HeavyThresholdBaseTicks, chargeSpeed);
            int fullChargeExtraTicks = CeilDiv(tuning.FullChargeExtraBaseTicks, chargeSpeed);

            bool charging = heldNow && heldTicks >= heavyThresholdTicks;

            float charge01 = 0f;
            if (charging)
            {
                int over = heldTicks - heavyThresholdTicks;

                if (over <= 0) charge01 = 0f;
                else if (fullChargeExtraTicks <= 0) charge01 = 1f;
                else
                {
                    float t = over / (float)fullChargeExtraTicks;
                    if (t < 0f) t = 0f;
                    if (t > 1f) t = 1f;
                    charge01 = t;
                }
            }

            return new ChargeState(charging, charge01, heavyThresholdTicks);
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