namespace Riftborne.Core.Config
{
    public readonly struct StatsToPhysicsTuning
    {
        public readonly float MinMoveSpeedMultiplier;
        public readonly float MaxMoveSpeedMultiplier;

        public StatsToPhysicsTuning(float minMoveSpeedMultiplier, float maxMoveSpeedMultiplier)
        {
            MinMoveSpeedMultiplier = minMoveSpeedMultiplier;
            MaxMoveSpeedMultiplier = maxMoveSpeedMultiplier;
        }
    }
}