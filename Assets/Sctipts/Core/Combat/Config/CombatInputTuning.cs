namespace Game.Core.Combat.Config
{
    public sealed class CombatInputTuning
    {
        public int AttackHoldTicksForHeavy;
        public int DefenseHoldTicksForBlock;
        public int InputBufferTicks;

        public float EvadeDeadzone; // 0..1

        public CombatInputTuning(
            int attackHoldTicksForHeavy,
            int defenseHoldTicksForBlock,
            int inputBufferTicks,
            float evadeDeadzone)
        {
            AttackHoldTicksForHeavy = attackHoldTicksForHeavy;
            DefenseHoldTicksForBlock = defenseHoldTicksForBlock;
            InputBufferTicks = inputBufferTicks;
            EvadeDeadzone = evadeDeadzone;
        }
    }
}
