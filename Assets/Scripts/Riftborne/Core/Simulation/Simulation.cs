using System.Collections.Generic;
using Riftborne.Core.Commands;
using Riftborne.Core.Model;
using Riftborne.Core.Physics.Abstractions;

namespace Riftborne.Core.Simulation
{
    public sealed class Simulation : ISimulation
    {
        private readonly GameState _state;
        private readonly ICommandDispatcher _dispatcher;
        private readonly SimulationParameters _parameters;

        // Optional physics dependencies (Null Object by DI or just null for pure-logic runs)
        private readonly IPhysicsWorld _physicsWorld;
        private readonly IBodyProvider<GameEntityId> _bodies;
        private readonly IGroundSensor _groundSensor;


        public Simulation(
            GameState state,
            ICommandDispatcher dispatcher,
            SimulationParameters parameters,
            IPhysicsWorld physicsWorld,
            IBodyProvider<GameEntityId> bodies,
            IGroundSensor groundSensor)
        {
            _state = state;
            _dispatcher = dispatcher;
            _parameters = parameters;
            _physicsWorld = physicsWorld;
            _bodies = bodies;
            _groundSensor = groundSensor;
        }
        public GameState State => _state;

        public void Step(int tick, IReadOnlyList<ICommand> commandsForTick)
        {
            // 1) Apply commands (they steer bodies, queue impulses, etc.)
            _dispatcher.Dispatch(commandsForTick);

            // 2) Step physics authoritatively for this tick
            _physicsWorld.Step(_parameters.TickDeltaTime, _parameters.PhysicsSubsteps);

            // 3) Mirror physical state back into GameState (view & logic)
            foreach (var kv in _state.Entities)
            {
                kv.Value.BeginTick();

                if (_bodies.TryGet(kv.Key, out var body))
                {
                    kv.Value.SetPose(body.X, body.Y, body.Vx, body.Vy, _groundSensor.IsGrounded(kv.Key));
                }
            }

            // 4) Sync tick
            _state.SetTick(tick + 1);
        }
    }
}
