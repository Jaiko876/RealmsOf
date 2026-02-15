using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Game.Unity.Input
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputAdapter : MonoBehaviour
    {
        private PlayerInputController _controller;
        private InputSnapshot _snapshot;

        [Inject]
        public void Construct(PlayerInputController controller)
        {
            _controller = controller;
        }

        public void OnMove(InputValue value)
        {
            var move = value.Get<Vector2>();

            _snapshot.MoveX = move.x;
            _snapshot.MoveY = move.y;

            _controller.SetSnapshot(_snapshot);
        }

        public void OnJump(InputValue value)
        {
            _snapshot.Jump = value.isPressed;
            _controller.SetSnapshot(_snapshot);
        }
    }
}
