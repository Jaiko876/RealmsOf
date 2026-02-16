namespace Riftborne.Core.Combat.Config
{
    public sealed class CombatResourceTuning
    {
        public float StaminaRegenPerSec { get; }
        public int StaminaRegenDelayTicks { get; }

        public float StaggerDecayPerSec { get; }
        public int StaggerBrokenWindowTicks { get; }

        public float VulnerableHpDamageMultiplier { get; }

        public CombatResourceTuning(
            float staminaRegenPerSec,
            int staminaRegenDelayTicks,
            float staggerDecayPerSec,
            int staggerBrokenWindowTicks,
            float vulnerableHpDamageMultiplier)
        {
            StaminaRegenPerSec = staminaRegenPerSec;
            StaminaRegenDelayTicks = staminaRegenDelayTicks;
            StaggerDecayPerSec = staggerDecayPerSec;
            StaggerBrokenWindowTicks = staggerBrokenWindowTicks;
            VulnerableHpDamageMultiplier = vulnerableHpDamageMultiplier;
        }
    }
}
