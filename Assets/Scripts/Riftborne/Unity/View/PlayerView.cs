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

            if (visualRoot == null)
                visualRoot = transform;

            // ВАЖНО: один раз связываем player -> avatar entity
            _gameState.PlayerAvatars.Set(_playerId, _entityId);

            // И гарантируем, что entity существует в стейте (чтобы не было null/KeyNotFound)
            _gameState.GetOrCreateEntity(_entityId);
        }

        private void LateUpdate()
        {
            var e = _gameState.GetOrCreateEntity(_entityId);

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
        }

    }
}
