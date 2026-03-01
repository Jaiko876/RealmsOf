namespace Riftborne.Core.Model.Animation
{
    public struct AnimationState
    {
        // One-shot trigger (event tick)
        public ActionState Action;
        public int ActionTick;
        public int ActionDurationTicks; // 0 means "unknown / not provided"

        // NEW: latched playback window (deterministic)
        public ActionState PlayingAction;       // None if nothing is considered "playing"
        public int PlayingStartTick;            // tick when playing started
        public int PlayingDurationTicks;        // duration in sim ticks (>0), 0 = not latched

        public sbyte Facing;

        public bool Grounded;
        public bool JustLanded;
        public bool Moving;

        public float Speed01;
        public float AirSpeed01;
        public float AirT;

        public bool Blocking;

        public bool HeavyCharging;
        public float Charge01;

        public float AttackAnimSpeed;
        public float ChargeAnimSpeed;
    }
}