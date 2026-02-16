using Riftborne.Core.Model;
using UnityEngine;
using VContainer;

namespace Riftborne.Unity.View
{
    public sealed class PlayerView : MonoBehaviour
    {
        [SerializeField] private int playerId = 0;
        [SerializeField] private int avatarEntityId = 0;

        [SerializeField] private Transform visualRoot;
        [SerializeField] private Animator animator;

        private GameState _gameState;
        private PlayerId _playerId;
        private GameEntityId _entityId;

        [Inject]
        public void Construct(GameState gameState)
        {
            _gameState = gameState;
        }

        private void Awake()
        {
            _playerId = new PlayerId(playerId);
            _entityId = new GameEntityId(avatarEntityId);

            if (visualRoot == null) visualRoot = transform;

            _gameState.PlayerAvatars.Set(_playerId, _entityId);
            _gameState.GetOrCreateEntity(_entityId);
        }

        private void LateUpdate()
        {
            var e = _gameState.GetOrCreateEntity(_entityId);

            // интерполяция позиции (как у тебя)
            var alpha = 1f;
            var fd = Time.fixedDeltaTime;
            if (fd > 0f)
            {
                alpha = (Time.time - Time.fixedTime) / fd;
                if (alpha < 0f) alpha = 0f;
                if (alpha > 1f) alpha = 1f;
            }

            var x = Mathf.Lerp(e.PrevX, e.X, alpha);
            var y = Mathf.Lerp(e.PrevY, e.Y, alpha);

            visualRoot.position = new Vector3(x, y, 0f);

            var facing = e.Facing; // -1 или +1
            var s = visualRoot.localScale;
            s.x = facing < 0 ? -Mathf.Abs(s.x) : Mathf.Abs(s.x);
            visualRoot.localScale = s;

            var vx = e.Vx;
            var vy = e.Vy;

            var speed01 = Mathf.Clamp01(Mathf.Abs(e.Vx) / 8f);
            var moveBlend = speed01 * 3f;

            animator.SetFloat("MoveBlendX", moveBlend);
            animator.SetBool("IsGrounded", e.Grounded); // если у тебя это уже есть
        }
    }
}