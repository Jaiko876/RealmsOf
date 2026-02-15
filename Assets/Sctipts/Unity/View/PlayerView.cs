using Game.Core.Model;
using UnityEngine;
using VContainer;

namespace Game.Unity.View
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
            // Можно брать напрямую entity (самый быстрый путь)
            var entity = _gameState.GetOrCreateEntity(_entityId);

            visualRoot.position = new Vector3(entity.X, entity.Y, 0f);
        }
    }
}
