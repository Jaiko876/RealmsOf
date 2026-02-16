using Riftborne.App.Commands;
using Riftborne.App.Time.Time;
using Riftborne.Core.Commands;
using Riftborne.Core.Model;
using UnityEngine;
using VContainer;

namespace Riftborne.Unity.Input
{
    public class PlayerInputController : MonoBehaviour
    {
        [SerializeField] private int controlledEntityId = 0;

        private GameEntityId Controlled => new GameEntityId(controlledEntityId);

        private ICommandQueue _commandQueue;
        private ITickClock _clock;

        private InputSnapshot _snapshot;

        private bool _prevJumpHeld;
        private bool _prevAttackHeld;
        private bool _prevDefenseHeld;

        // hold tracking (ticks)
        private int _attackHoldStartTick = -1;
        private bool _attackConsumedAsHeavy;

        private int _defenseHoldStartTick = -1;
        private bool _defenseConsumedAsBlock;

        [Inject]
        public void Construct(
            ICommandQueue commandQueue,
            ITickClock clock)
        {
            _commandQueue = commandQueue;
            _clock = clock;
        }

        public void SetMove(float x, float y)
        {
            _snapshot.MoveX = x;
            _snapshot.MoveY = y;
        }

        public void SetJumpHeld(bool held)
        {
            _snapshot.JumpHeld = held;

            if (held && !_prevJumpHeld)
                _snapshot.JumpPressed = true;

            _prevJumpHeld = held;
        }

        // --- Combat input setters ---
        

        public void FlushForTick(int tick)
        {
            // 1) Movement каждую физику
            var s = _snapshot;

            _commandQueue.Enqueue(new MoveCommand(
                tick,
                Controlled,
                s.MoveX,
                s.MoveY,
                s.JumpPressed,
                s.JumpHeld
            ));

            // 4) Consume one-frame edges
            _snapshot.JumpPressed = false;
            _snapshot.AttackPressed = false;
            _snapshot.DefensePressed = false;
            _snapshot.EvadePressed = false;
        }
    }
}
