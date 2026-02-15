using Game.App.Commands;
using Game.App.Time;
using Game.Core.Commands;
using Game.Core.Model;
using Game.Core.Combat.Abilities;
using UnityEngine;
using VContainer;

namespace Game.Unity.Input
{
    public class PlayerInputController : MonoBehaviour
    {
        [SerializeField] private int controlledEntityId = 0;

        private GameEntityId Controlled => new GameEntityId(controlledEntityId);

        private ICommandQueue _commandQueue;
        private ITickClock _clock;

        private InputSnapshot _snapshot;
        private bool _prevJumpHeld;

        [Inject]
        public void Construct(ICommandQueue commandQueue, ITickClock clock)
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

        public void UseAbility(AbilitySlot slot)
        {
            var tick = _clock.CurrentTick;

            _commandQueue.Enqueue(
                new UseAbilityCommand(
                    tick,
                    Controlled,
                    slot
                )
            );
        }

        public void FlushForTick(int tick)
        {
            var s = _snapshot;

            _commandQueue.Enqueue(new MoveCommand(
                tick,
                Controlled,
                s.MoveX,
                s.MoveY,
                s.JumpPressed,
                s.JumpHeld
            ));

            _snapshot.JumpPressed = false;
        }
    }
}
