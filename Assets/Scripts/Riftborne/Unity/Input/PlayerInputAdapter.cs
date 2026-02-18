using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Riftborne.Unity.Input
{
    [RequireComponent(typeof(PlayerInputController))]
    public sealed class PlayerInputAdapter : MonoBehaviour
    {
        private PlayerInputController _controller;
        private PlayerControls _controls;

        private Action<InputAction.CallbackContext> _onMovePerformed;
        private Action<InputAction.CallbackContext> _onMoveCanceled;

        private Action<InputAction.CallbackContext> _onJumpStarted;
        private Action<InputAction.CallbackContext> _onJumpCanceled;

        private Action<InputAction.CallbackContext> _onAttackStarted;
        private Action<InputAction.CallbackContext> _onAttackCanceled;

        private Action<InputAction.CallbackContext> _onDefenseStarted;
        private Action<InputAction.CallbackContext> _onDefenseCanceled;

        private Action<InputAction.CallbackContext> _onEvadeStarted;
        
        

        private bool _initialized;

        [Inject]
        public void Construct(PlayerInputController controller)
        {
            _controller = controller;
        }

        private void OnEnable()
        {
            EnsureInitialized();
            _controls.Enable();
        }

        private void OnDisable()
        {
            if (_controls != null)
                _controls.Disable();
        }

        private void OnDestroy()
        {
            if (_controls == null)
                return;

            // отписка
            var g = _controls.Gameplay;

            g.Move.performed -= _onMovePerformed;
            g.Move.canceled  -= _onMoveCanceled;

            g.Jump.started   -= _onJumpStarted;
            g.Jump.canceled  -= _onJumpCanceled;

            g.Attack.started -= _onAttackStarted;
            g.Attack.canceled-= _onAttackCanceled;

            g.Defense.started-= _onDefenseStarted;
            g.Defense.canceled-= _onDefenseCanceled;

            g.Evade.started  -= _onEvadeStarted;

            _controls.Dispose();
            _controls = null;
        }

        private void EnsureInitialized()
        {
            if (_initialized)
                return;

            if (_controller == null)
            {
                // fallback, если DI по каким-то причинам не сработал
                _controller = GetComponent<PlayerInputController>();
                if (_controller == null)
                {
                    Debug.LogError("PlayerInputAdapter: PlayerInputController is missing.");
                    enabled = false;
                    return;
                }
            }

            _controls = new PlayerControls();
            var g = _controls.Gameplay;

            _onMovePerformed = ctx =>
            {
                Vector2 v = ctx.ReadValue<Vector2>();
                _controller.SetMove(v.x, v.y);
            };
            _onMoveCanceled = ctx => _controller.SetMove(0f, 0f);

            _onJumpStarted  = ctx => _controller.SetJumpHeld(true);
            _onJumpCanceled = ctx => _controller.SetJumpHeld(false);

            _onAttackStarted  = ctx => _controller.SetAttackHeld(true);
            _onAttackCanceled = ctx => _controller.SetAttackHeld(false);

            _onDefenseStarted  = ctx => _controller.SetDefenseHeld(true);
            _onDefenseCanceled = ctx => _controller.SetDefenseHeld(false);

            _onEvadeStarted = ctx => _controller.SetEvadePressed();

            // подписка
            g.Move.performed += _onMovePerformed;
            g.Move.canceled  += _onMoveCanceled;

            g.Jump.started   += _onJumpStarted;
            g.Jump.canceled  += _onJumpCanceled;

            g.Attack.started += _onAttackStarted;
            g.Attack.canceled+= _onAttackCanceled;

            g.Defense.started+= _onDefenseStarted;
            g.Defense.canceled+= _onDefenseCanceled;

            g.Evade.started  += _onEvadeStarted;

            _initialized = true;
        }
    }
}
