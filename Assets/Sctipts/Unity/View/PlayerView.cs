using Game.Core.Model;
using UnityEngine;
using VContainer;

namespace Game.Unity.View
{
    public sealed class PlayerView : MonoBehaviour
    {
        [SerializeField] private int playerId = 0;

        // Сюда укажи объект со SpriteRenderer/Animator (обычно child).
        [SerializeField] private Transform visualRoot;

        private GameState _gameState;
        private PlayerId _id;

        [Inject]
        public void Construct(GameState gameState)
        {
            _gameState = gameState;
        }

        private void Awake()
        {
            _id = new PlayerId(playerId);

            // Если не задано — будем двигать себя (но это лучше, чем падать в null)
            if (visualRoot == null)
                visualRoot = transform;
        }

        private void LateUpdate()
        {
            var player = _gameState.GetOrCreatePlayer(_id);

            // Двигаем только визуал. Физическое тело пусть живет своей жизнью.
            visualRoot.position = new Vector3(player.X, player.Y, 0f);
        }
    }
}
