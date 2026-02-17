using System.Collections.Generic;
using Riftborne.Core.Commands;
using Riftborne.Core.Model;
using Riftborne.Core.Physics.Abstractions;
using Riftborne.Core.Systems;

namespace Riftborne.Core.Simulation
{
    public sealed class Simulation : ISimulation
    {
        private readonly GameState _state;
        private readonly ICommandDispatcher _dispatcher;
        private readonly SimulationParameters _parameters;
        
        private readonly IPhysicsWorld _physicsWorld;

        private readonly ITickPipeline _tickPipeline;

        public Simulation(
            GameState state,
            ICommandDispatcher dispatcher,
            SimulationParameters parameters,
            IPhysicsWorld physicsWorld,
            ITickPipeline pipeline)
        {
            _state = state;
            _dispatcher = dispatcher;
            _parameters = parameters;
            _physicsWorld = physicsWorld;
            _tickPipeline = pipeline;
        }

        
        public GameState State => _state;

        public void Step(int tick, IReadOnlyList<ICommand> commandsForTick)
        {
            // 0) Начало тика для сущностей (если нужно)
            foreach (var kv in _state.Entities)
                kv.Value.BeginTick();

            // 1) Разобрать команды → заполнить stores (input, intents, etc.)
            _dispatcher.Dispatch(commandsForTick);

            // 2) Pre-physics systems: применяем motor, импульсы, etc.
            var pre = _tickPipeline.PrePhysics;
            for (int i = 0; i < pre.Count; i++)
                pre[i].Tick(tick);

            // 3) Authoritative physics
            _physicsWorld.Step(_parameters.TickDeltaTime, _parameters.PhysicsSubsteps);

            // 4) Post-physics systems: синк стейта, ресурсы, события, и т.п.
            var post = _tickPipeline.PostPhysics;
            for (int i = 0; i < post.Count; i++)
                post[i].Tick(tick);

            // 5) Sync tick
            _state.SetTick(tick + 1);
        }

    }
}
