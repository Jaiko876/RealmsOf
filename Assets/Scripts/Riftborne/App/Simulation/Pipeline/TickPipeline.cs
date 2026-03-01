using System.Collections.Generic;
using Riftborne.App.Animation.Systems;
using Riftborne.App.Combat.Systems;
using Riftborne.Core.Systems.PostPhysicsTickSystems;
using Riftborne.Core.Systems.PrePhysicsTickSystems;

namespace Riftborne.App.Simulation.Pipeline
{
    public sealed class TickPipeline : ITickPipeline
    {
        public IReadOnlyList<IPrePhysicsTickSystem> PrePhysics { get; }
        public IReadOnlyList<IPostPhysicsTickSystem> PostPhysics { get; }

        public TickPipeline(
            MotorPrePhysicsTickSystem motor,
            CombatDodgeMovementPrePhysicsSystem dodgeMove,
            PostPhysicsStateSyncSystem sync,
            StatsInitPostPhysicsSystem statsInit,
            StatsRegenPostPhysicsSystem statsRegen,
            StatsApplyDeltasPostPhysicsSystem statsDeltas,
            StatsEffectsTickPostPhysicsSystem statsEffects,
            StatsRebuildModifiersPostPhysicsSystem statsRebuildModifiers,
            CombatActionsPostPhysicsSystem combat,
            AnimationStatePostPhysicsSystem animation)
        {
            PrePhysics = new IPrePhysicsTickSystem[]
            {
                motor,
                dodgeMove
            };

            PostPhysics = new IPostPhysicsTickSystem[]
            {
                sync,
                statsInit,
                statsEffects,
                statsRebuildModifiers,
                combat,
                statsRegen,
                statsDeltas,
                animation
            };
        }
    }
}