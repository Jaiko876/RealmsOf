namespace Riftborne.Core.Config
{
    public readonly struct DefenseInputTuning
    {
        // <= this => tap => parry (on release)
        public readonly int ParryMaxTapTicks;

        // >= this => start blocking while still held
        public readonly int BlockStartTicks;

        public DefenseInputTuning(int parryMaxTapTicks, int blockStartTicks)
        {
            ParryMaxTapTicks = parryMaxTapTicks < 0 ? 0 : parryMaxTapTicks;
            BlockStartTicks = blockStartTicks < 0 ? 0 : blockStartTicks;
        }
    }
}