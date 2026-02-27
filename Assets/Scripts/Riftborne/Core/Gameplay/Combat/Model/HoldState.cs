namespace Riftborne.Core.Gameplay.Combat.Model
{
    public readonly struct HoldState
    {
        public readonly bool IsHeld;
        public readonly int HeldTicks;
        public readonly bool ReleasedThisTick;

        public HoldState(bool isHeld, int heldTicks, bool releasedThisTick)
        {
            IsHeld = isHeld;
            HeldTicks = heldTicks;
            ReleasedThisTick = releasedThisTick;
        }
    }
}