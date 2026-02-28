using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Riftborne.App.Commands.Queue;
using Riftborne.App.Simulation;
using Riftborne.App.Time;
using VContainer;
using VContainer.Unity;

namespace Riftborne.Unity.Bootstrap
{
    public sealed class GameLoop : IStartable
    {
        private readonly ITickClock _clock;
        private readonly ICommandQueue _queue;
        private readonly ISimulation _simulation;

        private readonly ITickCommandSource[] _sources;

        [Inject]
        public GameLoop(
            ITickClock clock,
            ICommandQueue queue,
            ISimulation simulation,
            IEnumerable<ITickCommandSource> sources)
        {
            _clock = clock;
            _queue = queue;
            _simulation = simulation;

            _sources = sources != null ? ToArray(sources) : new ITickCommandSource[0];
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

                for (int i = 0; i < _sources.Length; i++)
                    _sources[i].ProduceCommandsForTick(tick);

                var batch = _queue.DequeueAllForTick(tick);
                _simulation.Step(tick, batch);

                _clock.Advance();
            }
        }

        private static ITickCommandSource[] ToArray(IEnumerable<ITickCommandSource> src)
        {
            var list = new List<ITickCommandSource>();
            foreach (var s in src)
            {
                if (s != null) list.Add(s);
            }
            return list.ToArray();
        }
    }
}