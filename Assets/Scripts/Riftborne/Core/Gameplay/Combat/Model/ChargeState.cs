namespace Riftborne.Core.Gameplay.Combat.Model
{

    public readonly struct ChargeState
    {
        public readonly bool Charging;
        public readonly float Charge01;
        public readonly int HeavyThresholdTicks;

        public ChargeState(bool charging, float charge01, int heavyThresholdTicks)
        {
            Charging = charging;
            Charge01 = charge01;
            HeavyThresholdTicks = heavyThresholdTicks;
        }
    }
}