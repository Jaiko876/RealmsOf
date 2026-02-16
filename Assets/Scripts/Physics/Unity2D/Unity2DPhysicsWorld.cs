using Game.Core.Physics.Abstractions;
using UnityEngine;

namespace Game.Physics.Unity2D
{

    public sealed class Unity2DPhysicsWorld : IPhysicsWorld
    {
        public void Step(float dt, int substeps)
        {
            if (substeps < 1) substeps = 1;
            if (substeps > 8) substeps = 8;

            var subDt = dt / substeps;
            for (int i = 0; i < substeps; i++)
                Physics2D.Simulate(subDt);
        }
    }
}
