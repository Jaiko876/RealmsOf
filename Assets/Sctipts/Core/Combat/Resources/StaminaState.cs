namespace Game.Core.Combat.Resources
{
    public struct StaminaState
    {
        public float Current;
        public int LastSpendTick;

        public StaminaState(float current, int lastSpendTick)
        {
            Current = current;
            LastSpendTick = lastSpendTick;
        }
    }
}
