namespace Riftborne.Core.Config
{
    public readonly struct CombatResourceTuning
    {
        public readonly int LightAttackStaminaCost;
        public readonly int HeavyChargeStartStaminaCost;
        public readonly int ParryStaminaCost;
        public readonly int DodgeStaminaCost;
        public readonly int BlockStaminaPerTick;

        public CombatResourceTuning(
            int lightAttackStaminaCost,
            int heavyChargeStartStaminaCost,
            int parryStaminaCost,
            int dodgeStaminaCost,
            int blockStaminaPerTick)
        {
            LightAttackStaminaCost = ClampNonNegative(lightAttackStaminaCost);
            HeavyChargeStartStaminaCost = ClampNonNegative(heavyChargeStartStaminaCost);
            ParryStaminaCost = ClampNonNegative(parryStaminaCost);
            DodgeStaminaCost = ClampNonNegative(dodgeStaminaCost);
            BlockStaminaPerTick = ClampNonNegative(blockStaminaPerTick);
        }

        public static CombatResourceTuning Default => new CombatResourceTuning(
            lightAttackStaminaCost: 8,
            heavyChargeStartStaminaCost: 14,
            parryStaminaCost: 6,
            dodgeStaminaCost: 18,
            blockStaminaPerTick: 1
        );

        private static int ClampNonNegative(int v) => v < 0 ? 0 : v;
    }
}