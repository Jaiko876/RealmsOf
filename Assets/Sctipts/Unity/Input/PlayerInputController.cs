using Game.App.Commands;
using Game.App.Time;
using Game.Core.Commands;
using Game.Core.Model;
using UnityEngine;
using VContainer;

namespace Game.Unity.Input
{
    public class PlayerInputController : MonoBehaviour
    {
        private ICommandQueue _commandQueue;
        private ITickClock _clock;

        private InputSnapshot _snapshot;

        [Inject]
        public void Construct(ICommandQueue commandQueue, ITickClock clock)
        {
            _commandQueue = commandQueue;
            _clock = clock;
        }

        public void SetSnapshot(InputSnapshot snapshot)
        {
            _snapshot = snapshot;
        }

        public void FlushForTick(int tick)
        {
            if (_snapshot.HasMovement)
            {
                var cmd = new MoveCommand(
                    tick,
                    PlayerId.Local,
                    _snapshot.MoveX,
                    _snapshot.MoveY
                );

                _commandQueue.Enqueue(cmd);
            }
        }
    }
}
