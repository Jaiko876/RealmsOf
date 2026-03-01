namespace Riftborne.Core.Config
{
    public interface IGameplayTuning
    {
        CombatInputTuning CombatInput { get; }
        CombatAnimationTuning CombatAnimation { get; }
        CombatActionsTuning CombatActions { get; }
        CombatHitTuning CombatHit { get; }
        CombatDamageTuning CombatDamage { get; }
        CombatResourceTuning CombatResources { get; }

        StatsToPhysicsTuning StatsToPhysics { get; }
        InputTuning Input { get; }
        PhysicsProbesTuning PhysicsProbes { get; }
        PhysicsWorldTuning PhysicsWorld { get; }
        DefenseInputTuning DefenseInput { get; }
    }
}