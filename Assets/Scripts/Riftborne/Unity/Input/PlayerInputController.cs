using Riftborne.App.Commands;
using Riftborne.App.Time.Time;
using Riftborne.Core.Input;
using Riftborne.Core.Model;
using UnityEngine;
using VContainer;

namespace Riftborne.Unity.Input
{
    public class PlayerInputController : MonoBehaviour
    {
        [SerializeField] private int controlledEntityId = 0;
        
        [SerializeField] private int heavyHoldTicks = 12; // при TickRate=50 это ~0.2с
        
        private GameEntityId Controlled => new(controlledEntityId);

        private ICommandQueue _commandQueue;
        private ITickClock _clock;

        private InputSnapshot _snapshot;

        private bool _attackHeldPrevTick;
        private int _attackHoldStartTick = -1;
        private bool _attackHeavyFired;
        
        private bool _prevJumpHeld;
        private bool _prevAttackHeld;
        private bool _prevDefenseHeld;

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
        
        public void SetAttackHeld(bool held)
        {
            _snapshot.AttackHeld = held;
        }


        public void SetDefenseHeld(bool held)
        {
            _snapshot.DefenseHeld = held;
            if (held && !_prevDefenseHeld)
                _snapshot.DefensePressed = true;
            _prevDefenseHeld = held;
        }

        public void SetEvadePressed()
        {
            _snapshot.EvadePressed = true;
        }

        public void FlushForTick(int tick)
        {
            var s = _snapshot;
            var buttons = InputButtons.None;

            if (s.JumpPressed) buttons |= InputButtons.JumpPressed;
            if (s.JumpHeld)    buttons |= InputButtons.JumpHeld;

            if (s.AttackHeld)         buttons |= InputButtons.AttackHeld;         // raw (может пригодиться позже)

            if (s.DefensePressed) buttons |= InputButtons.DefensePressed;
            if (s.DefenseHeld)    buttons |= InputButtons.DefenseHeld;

            if (s.EvadePressed) buttons |= InputButtons.EvadePressed;

            _commandQueue.Enqueue(new InputCommand(
                tick,
                Controlled,
                s.MoveX,
                s.MoveY,
                buttons
            ));

            // edges — в ноль
            _snapshot.JumpPressed = false;
            _snapshot.DefensePressed = false;
            _snapshot.EvadePressed = false;
        }
    }
}
