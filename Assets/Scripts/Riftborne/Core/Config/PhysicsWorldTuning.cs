namespace Riftborne.Core.Config
{
    public readonly struct PhysicsWorldTuning
    {
        public readonly int MaxSubSteps;

        public PhysicsWorldTuning(int maxSubSteps)
        {
            MaxSubSteps = maxSubSteps;
        }
    }
}