namespace Riftborne.Core.Config
{
    public readonly struct CombatInputTuning
    {
        public readonly int HeavyThresholdBaseTicks;
        public readonly int FullChargeExtraBaseTicks;

        public readonly int LightCooldownBaseTicks;
        public readonly int HeavyCooldownBaseTicks;

        public readonly float MinAttackSpeed;
        public readonly float MaxAttackSpeed;

        public readonly float MinChargeSpeed;
        public readonly float MaxChargeSpeed;

        public CombatInputTuning(
            int heavyThresholdBaseTicks,
            int fullChargeExtraBaseTicks,
            int lightCooldownBaseTicks,
            int heavyCooldownBaseTicks,
            float minAttackSpeed,
            float maxAttackSpeed,
            float minChargeSpeed,
            float maxChargeSpeed)
        {
            HeavyThresholdBaseTicks = heavyThresholdBaseTicks;
            FullChargeExtraBaseTicks = fullChargeExtraBaseTicks;
            LightCooldownBaseTicks = lightCooldownBaseTicks;
            HeavyCooldownBaseTicks = heavyCooldownBaseTicks;
            MinAttackSpeed = minAttackSpeed;
            MaxAttackSpeed = maxAttackSpeed;
            MinChargeSpeed = minChargeSpeed;
            MaxChargeSpeed = maxChargeSpeed;
        }
    }
}