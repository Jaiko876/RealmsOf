using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Riftborne.Unity.Input
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputAdapter : MonoBehaviour
    {
        private PlayerInputController _controller;

        [Inject]
        public void Construct(PlayerInputController controller)
        {
            _controller = controller;
        }

        public void OnMove(InputValue value)
        {
            var move = value.Get<Vector2>();
            _controller.SetMove(move.x, move.y);
        }

        public void OnJump(InputValue value)
        {
            _controller.SetJumpHeld(value.isPressed);
        }

        // West button: Attack
        public void OnAttack(InputValue value)
        {
            _controller.SetAttackHeld(value.isPressed);
        }

        // North button: Defense
        public void OnDefense(InputValue value)
        {
            _controller.SetDefenseHeld(value.isPressed);
        }

        // East button: Evade
        public void OnEvade(InputValue value)
        {
            if (value.isPressed)
                _controller.OnEvadePressed();
        }
    }
}
