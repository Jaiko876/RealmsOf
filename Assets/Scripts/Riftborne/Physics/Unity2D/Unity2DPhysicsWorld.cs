using System;
using Riftborne.Core.Config;
using Riftborne.Core.Physics.Abstractions;
using UnityEngine;

namespace Riftborne.Physics.Unity2D
{
    public sealed class Unity2DPhysicsWorld : IPhysicsWorld
    {
        private readonly int _maxSubSteps;

        public Unity2DPhysicsWorld(IGameplayTuning tuning)
        {
            if (tuning == null) throw new ArgumentNullException(nameof(tuning));
            _maxSubSteps = tuning.PhysicsWorld.MaxSubSteps;
            if (_maxSubSteps < 1) _maxSubSteps = 1;
        }

        public void Step(float dt, int substeps)
        {
            if (substeps < 1) substeps = 1;
            if (substeps > _maxSubSteps) substeps = _maxSubSteps;

            var subDt = dt / substeps;
            for (int i = 0; i < substeps; i++)
                Physics2D.Simulate(subDt);
        }
    }
}