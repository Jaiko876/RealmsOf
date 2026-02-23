namespace Riftborne.Core.Config
{
    public interface IGameplayTuning
    {
        CombatInputTuning CombatInput { get; }
        StatsToPhysicsTuning StatsToPhysics { get; }
        InputTuning Input { get; }
        PhysicsProbesTuning PhysicsProbes { get; }
        PhysicsWorldTuning PhysicsWorld { get; }
    }
}