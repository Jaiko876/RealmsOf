using System;
using Riftborne.Core.Model;
using Riftborne.Core.Physics.Abstractions;

namespace Riftborne.Core.Systems.PostPhysicsTickSystems
{
    public sealed class PostPhysicsStateSyncSystem : IPostPhysicsTickSystem
    {
        private readonly GameState _state;
        private readonly IBodyProvider<GameEntityId> _bodies;
        private readonly IGroundSensor _groundSensor;

        public PostPhysicsStateSyncSystem(
            GameState state,
            IBodyProvider<GameEntityId> bodies,
            IGroundSensor groundSensor)
        {
            _state = state;
            _bodies = bodies;
            _groundSensor = groundSensor;
        }

        public void Tick(int tick)
        {
            foreach (var kv in _state.Entities)
            {
                var id = kv.Key;
                var e = kv.Value;

                if (!_bodies.TryGet(id, out var body))
                    continue;

                var grounded = _groundSensor.IsGrounded(id);

                e.SetPose(body.X, body.Y, body.Vx, body.Vy, grounded);
            }
        }
    }
}