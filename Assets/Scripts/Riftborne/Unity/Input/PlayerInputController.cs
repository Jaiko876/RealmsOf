using System;
using Riftborne.App.Commands.Queue;
using Riftborne.Core.Input;
using Riftborne.Core.Input.Commands;
using Riftborne.Core.Model;
using Riftborne.Unity.Bootstrap;
using UnityEngine;
using VContainer;

namespace Riftborne.Unity.Input
{
    public sealed class PlayerInputController : MonoBehaviour, ITickCommandSource
    {
        [Header("Bind (Local Player)")]
        [SerializeField] private int playerId = 0;

        [Header("Debug override (-1 = use PlayerAvatarMap)")]
        [SerializeField] private int overrideEntityId = -1;

        private ICommandQueue _commandQueue;
        private GameState _state;

        private InputSnapshot _snapshot;

        private bool _prevJumpHeld;
        private bool _prevDefenseHeld;

        private PlayerId _playerId;

        [Inject]
        public void Construct(ICommandQueue commandQueue, GameState state)
        {
            _commandQueue = commandQueue ?? throw new ArgumentNullException(nameof(commandQueue));
            _state = state ?? throw new ArgumentNullException(nameof(state));
        }

        private void Awake()
        {
            _playerId = new PlayerId(playerId);
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

        public void ProduceCommandsForTick(int tick)
        {
            FlushForTick(tick);
        }

        private void FlushForTick(int tick)
        {
            if (_commandQueue == null || _state == null)
                return;

            if (!TryResolveControlledEntity(out var controlled))
            {
                _snapshot.ClearEdges();
                return;
            }

            InputButtons buttons = InputButtons.None;

            if (_snapshot.JumpPressed) buttons |= InputButtons.JumpPressed;
            if (_snapshot.JumpHeld)    buttons |= InputButtons.JumpHeld;

            if (_snapshot.AttackHeld)  buttons |= InputButtons.AttackHeld;

            if (_snapshot.DefensePressed) buttons |= InputButtons.DefensePressed;
            if (_snapshot.DefenseHeld)    buttons |= InputButtons.DefenseHeld;

            if (_snapshot.EvadePressed) buttons |= InputButtons.EvadePressed;

            _commandQueue.Enqueue(new InputCommand(
                tick: tick,
                entityId: controlled,
                dx: _snapshot.MoveX,
                dy: _snapshot.MoveY,
                buttons: buttons
            ));

            _snapshot.ClearEdges();
        }

        private bool TryResolveControlledEntity(out GameEntityId id)
        {
            if (overrideEntityId >= 0)
            {
                id = new GameEntityId(overrideEntityId);
                return true;
            }

            if (_state.PlayerAvatars.TryGet(_playerId, out id))
                return true;

            id = default;
            return false;
        }
    }
}