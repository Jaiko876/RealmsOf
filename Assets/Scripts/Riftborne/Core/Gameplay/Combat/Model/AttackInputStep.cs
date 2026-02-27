namespace Riftborne.Core.Gameplay.Combat.Model
{
    public readonly struct AttackInputStep
    {
        public readonly HoldState Hold;
        public readonly ChargeState Charge;
        public readonly ReleaseDecision Release; // None if not released this tick

        public AttackInputStep(HoldState hold, ChargeState charge, ReleaseDecision release)
        {
            Hold = hold;
            Charge = charge;
            Release = release;
        }
    }
}