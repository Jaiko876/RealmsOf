namespace Riftborne.Core.Config
{
    public readonly struct CombatAnimationTuning
    {
        public readonly int LightAttackDurationBaseTicks;
        public readonly int HeavyAttackDurationBaseTicks;

        public CombatAnimationTuning(
            int lightAttackDurationBaseTicks,
            int heavyAttackDurationBaseTicks)
        {
            LightAttackDurationBaseTicks = lightAttackDurationBaseTicks;
            HeavyAttackDurationBaseTicks = heavyAttackDurationBaseTicks;
        }
    }
}