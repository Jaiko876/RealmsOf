using System.Collections.Generic;
using Riftborne.Core.Abstractions;
using Riftborne.Core.Combat.Abilities;
using Riftborne.Core.Combat.Health;
using Riftborne.Core.Combat.Resources;
using Riftborne.Core.Model;
using Riftborne.Core.Physics.Abstractions;
using Riftborne.Core.Stats;

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

        private readonly IHealthTickSystem _healthTickSystem;

        private readonly StatResolver _stats;

        private readonly ICombatResourceTickSystem _combatResourceTickSystem;

        private readonly ICombatActionTickSystem _combatActionTickSystem;


        public Simulation(
            GameState state,
            ICommandDispatcher dispatcher,
            SimulationParameters parameters,
            IPhysicsWorld physicsWorld,
            IBodyProvider<GameEntityId> bodies,
            IHealthTickSystem healthTickSystem,
            StatResolver stats,
            ICombatResourceTickSystem combatResourceTickSystem,
            ICombatActionTickSystem combatActionTickSystem)
        {
            _state = state;
            _dispatcher = dispatcher;
            _parameters = parameters;
            _physicsWorld = physicsWorld;
            _bodies = bodies;
            _healthTickSystem = healthTickSystem;
            _stats = stats;
            _combatResourceTickSystem = combatResourceTickSystem;
            _combatActionTickSystem = combatActionTickSystem;
        }
        public GameState State => _state;

        public void Step(int tick, IReadOnlyList<ICommand> commandsForTick)
        {
            _stats.SetTick(tick);
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
                    kv.Value.SetPose(body.X, body.Y, body.Vx, body.Vy);
                }
            }

            _healthTickSystem.Tick(tick, _parameters.TickDeltaTime);
            _combatResourceTickSystem.Tick(tick, _parameters.TickDeltaTime);
            _combatActionTickSystem.Tick(tick);

            // 4) Sync tick
            _state.SetTick(tick + 1);
        }
    }
}
