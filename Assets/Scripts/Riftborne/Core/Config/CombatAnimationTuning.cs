namespace Riftborne.Core.Config
{
    public readonly struct CombatAnimationTuning
    {
        public readonly int LightAttackDurationBaseTicks;
        public readonly int HeavyAttackDurationBaseTicks;

        /// <summary>
        /// Clamp for computed animator playback speed on the View side (defensive).
        /// Core does not use this, but it is convenient to keep in one tuning asset.
        /// </summary>
        public readonly float MinAnimatorSpeed;
        public readonly float MaxAnimatorSpeed;

        public CombatAnimationTuning(
            int lightAttackDurationBaseTicks,
            int heavyAttackDurationBaseTicks,
            float minAnimatorSpeed,
            float maxAnimatorSpeed)
        {
            LightAttackDurationBaseTicks = lightAttackDurationBaseTicks;
            HeavyAttackDurationBaseTicks = heavyAttackDurationBaseTicks;
            MinAnimatorSpeed = minAnimatorSpeed;
            MaxAnimatorSpeed = maxAnimatorSpeed;
        }
    }
}