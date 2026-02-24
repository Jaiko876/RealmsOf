using System.Collections.Generic;
using Riftborne.App.Animation.System;
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
            PostPhysicsStateSyncSystem sync,
            StatsInitPostPhysicsSystem statsInit,
            StatsRegenPostPhysicsSystem statsRegen,
            StatsApplyDeltasPostPhysicsSystem statsDeltas,
            StatsEffectsTickPostPhysicsSystem statsEffects,
            StatsRebuildModifiersPostPhysicsSystem statsRebuildModifiers,
            AnimationStatePostPhysicsSystem animation)
        {
            PrePhysics = new IPrePhysicsTickSystem[] { motor };

            PostPhysics = new IPostPhysicsTickSystem[]
            {
                sync,
                statsInit,
                statsEffects,
                statsRebuildModifiers,
                statsRegen,
                statsDeltas,
                animation
            };
        }
    }
}