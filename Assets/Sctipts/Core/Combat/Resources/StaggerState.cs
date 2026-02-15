namespace Game.Core.Combat.Resources
{
    public struct StaggerState
    {
        public float Current;
        public bool IsBroken;
        public int VulnerableUntilTick;

        public StaggerState(float current, bool isBroken, int vulnerableUntilTick)
        {
            Current = current;
            IsBroken = isBroken;
            VulnerableUntilTick = vulnerableUntilTick;
        }
    }
}
