using System.Collections.Generic;
using Riftborne.Core.Systems.PostPhysicsTickSystems;
using Riftborne.Core.Systems.PrePhysicsTickSystems;

namespace Riftborne.App.Simulation.Pipeline
{
    public interface ITickPipeline
    {
        IReadOnlyList<IPrePhysicsTickSystem> PrePhysics { get; }
        IReadOnlyList<IPostPhysicsTickSystem> PostPhysics { get; }
    }

}