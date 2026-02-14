using UnityEngine;

namespace Game.Unity.Input
{
    [RequireComponent(typeof(PlayerInputController))]
    public class KeyboardMoveInput : MonoBehaviour
    {
        private PlayerInputController _controller;

        private void Awake()
        {
            _controller = GetComponent<PlayerInputController>();
        }

        private void Update()
        {
            Vector2 move = Vector2.zero;

            // Новый Input System. Без using UnityEngine.InputSystem; чтобы не ловить конфликты имен.
            UnityEngine.InputSystem.Keyboard keyboard = UnityEngine.InputSystem.Keyboard.current;

            if (keyboard != null)
            {
                if (keyboard.wKey.isPressed) move.y += 1f;
                if (keyboard.sKey.isPressed) move.y -= 1f;
                if (keyboard.aKey.isPressed) move.x -= 1f;
                if (keyboard.dKey.isPressed) move.x += 1f;
            }

            if (move.sqrMagnitude > 1f)
            {
                move.Normalize();
            }

            _controller.SetMove(move);
        }
    }
}
