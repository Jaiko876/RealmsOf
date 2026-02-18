using System.Collections.Generic;
using Riftborne.Core.Systems.PostPhysicsTickSystems;
using Riftborne.Core.Systems.PrePhysicsTickSystems;

namespace Riftborne.Core.Systems
{
    public sealed class TickPipeline : ITickPipeline
    {
        public IReadOnlyList<IPrePhysicsTickSystem> PrePhysics { get; }
        public IReadOnlyList<IPostPhysicsTickSystem> PostPhysics { get; }

        public TickPipeline(
            MotorPrePhysicsTickSystem motor,
            AnimationStatePostPhysicsSystem animationStatePostPhysicsSystem,
            PostPhysicsStateSyncSystem sync)
        {
            PrePhysics = new IPrePhysicsTickSystem[]
            {
                motor
            };

            PostPhysics = new IPostPhysicsTickSystem[]
            {
                sync,
                animationStatePostPhysicsSystem
            };
        }
    }
}