using Game.Core.Model;
using UnityEngine;
using VContainer;

namespace Game.Unity.View
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private int playerId = 0;

        private GameState _gameState;

        [Inject]
        public void Construct(GameState gameState)
        {
            _gameState = gameState;
        }

        private void LateUpdate()
        {
            var id = new PlayerId(playerId);

            var player = _gameState.GetOrCreatePlayer(id);

            transform.position = new Vector3(player.X, player.Y, 0f);
        }

    }
}
