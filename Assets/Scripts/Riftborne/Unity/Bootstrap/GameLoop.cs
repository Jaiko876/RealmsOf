using Cysharp.Threading.Tasks;
using Riftborne.App.Commands;
using Riftborne.App.Time.Time;
using Riftborne.Core.Simulation;
using Riftborne.Core.Stats;
using Riftborne.Unity.Input;
using VContainer;
using VContainer.Unity;

namespace Riftborne.Unity.Bootstrap
{
    public class GameLoop : IStartable
    {
        private readonly ITickClock _clock;
        private readonly ICommandQueue _queue;
        private readonly ISimulation _simulation;
        private readonly PlayerInputController _input;

        private readonly StatResolver _stats;

        [Inject]
        public GameLoop(
            ITickClock clock,
            ICommandQueue queue,
            ISimulation simulation,
            PlayerInputController input,
            StatResolver stats)
        {
            _clock = clock;
            _queue = queue;
            _simulation = simulation;
            _input = input;
            _stats = stats;
        }


        public void Start()
        {
            Run().Forget();
        }

        private async UniTaskVoid Run()
        {
            while (true)
            {
                await UniTask.WaitForFixedUpdate();

                var tick = _clock.CurrentTick;

                _input.FlushForTick(tick);

                var batch = _queue.DequeueAllForTick(tick);
                _simulation.Step(tick, batch);

                _clock.Advance();
            }
        }
    }
}
