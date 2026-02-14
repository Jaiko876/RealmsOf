using Game.Domain.Commands;
using Game.Domain.Model;
using Game.Infrastructure.Commands;
using Game.Infrastructure.Time;
using UnityEngine;
using VContainer;

namespace Game.Presentation.Input
{

    public sealed class PlayerInputController : MonoBehaviour
    {
        [SerializeField] private int playerId = 0;

        private ICommandQueue _queue = null!;
        private ITickClock _clock = null!;

        private Vector2 _move;

        [Inject]
        public void Construct(ICommandQueue queue, ITickClock clock)
        {
            _queue = queue;
            _clock = clock;
        }

        public void SetMove(Vector2 move) => _move = move;

        private void Update()
        {
            if (_move == Vector2.zero) return;

            _queue.Enqueue(new MoveCommand(
                _clock.CurrentTick,
                new PlayerId(playerId),
                _move.x,
                _move.y
            ));
        }
    }
}