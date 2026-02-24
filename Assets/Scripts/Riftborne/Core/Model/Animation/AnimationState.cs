namespace Riftborne.Core.Model.Animation
{
    public struct AnimationState
    {
        public ActionState Action;

        // tick when Action was set (one-shot)
        public int ActionTick;

        // NEW: authoritative duration of the triggered Action in simulation ticks.
        // 0 means "unknown / not provided".
        public int ActionDurationTicks;

        public sbyte Facing;

        public bool Grounded;
        public bool JustLanded;
        public bool Moving;

        public float Speed01;
        public float AirSpeed01;
        public float AirT;

        public bool HeavyCharging;
        public float Charge01;

        public float AttackAnimSpeed;
        public float ChargeAnimSpeed;
    }
}