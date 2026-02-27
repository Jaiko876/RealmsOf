using System;
using Riftborne.App.Commands.Queue;
using Riftborne.Core.Input;
using Riftborne.Core.Input.Commands;
using Riftborne.Core.Model;
using UnityEngine;
using VContainer;

namespace Riftborne.Unity.Input
{
    public sealed class PlayerInputController : MonoBehaviour
    {
        [SerializeField] private int controlledEntityId = 0;

        private ICommandQueue _commandQueue;

        private InputSnapshot _snapshot;
        private bool _prevJumpHeld;
        private bool _prevDefenseHeld;

        private GameEntityId Controlled => new GameEntityId(controlledEntityId);

        [Inject]
        public void Construct(ICommandQueue commandQueue)
        {
            _commandQueue = commandQueue ?? throw new ArgumentNullException(nameof(commandQueue));
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
            if (_commandQueue == null)
                throw new InvalidOperationException("PlayerInputController is not constructed (DI failed?).");

            InputButtons buttons = InputButtons.None;

            if (_snapshot.JumpPressed) buttons |= InputButtons.JumpPressed;
            if (_snapshot.JumpHeld)    buttons |= InputButtons.JumpHeld;

            if (_snapshot.AttackHeld)  buttons |= InputButtons.AttackHeld;

            if (_snapshot.DefensePressed) buttons |= InputButtons.DefensePressed;
            if (_snapshot.DefenseHeld)    buttons |= InputButtons.DefenseHeld;

            if (_snapshot.EvadePressed) buttons |= InputButtons.EvadePressed;

            _commandQueue.Enqueue(new InputCommand(
                tick: tick,
                entityId: Controlled,
                dx: _snapshot.MoveX,
                dy: _snapshot.MoveY,
                buttons: buttons
            ));

            _snapshot.ClearEdges();
        }
    }
}