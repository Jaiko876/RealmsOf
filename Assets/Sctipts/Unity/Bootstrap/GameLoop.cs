using Cysharp.Threading.Tasks;
using Game.Core.Simulation;
using Game.App.Commands;
using Game.App.Time;
using VContainer.Unity;

namespace Game.Unity.Bootstrap
{

    public sealed class GameLoop : IStartable
    {
        private readonly ISimulation _simulation;
        private readonly ICommandQueue _commands;
        private readonly ITickClock _clock;

        public GameLoop(ISimulation simulation, ICommandQueue commands, ITickClock clock)
        {
            _simulation = simulation;
            _commands = commands;
            _clock = clock;
        }

        public void Start()
        {
            RunAsync().Forget();
        }

        private async UniTaskVoid RunAsync()
        {
            while (true)
            {
                // Ждём именно FixedUpdate. Это убирает дрейф от Delay(ms).
                await UniTask.WaitForFixedUpdate();

                var tick = _clock.CurrentTick;
                var batch = _commands.DequeueAllForTick(tick);

                _simulation.Step(tick, batch);
                _clock.Advance();
            }
        }
    }
}
