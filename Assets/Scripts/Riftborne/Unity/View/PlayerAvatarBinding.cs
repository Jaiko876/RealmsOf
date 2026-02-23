using Riftborne.Core.Model;
using UnityEngine;
using VContainer;

namespace Riftborne.Unity.View
{
    public sealed class PlayerAvatarBinding : MonoBehaviour
    {
        [SerializeField] private int playerId = 0;
        [SerializeField] private int avatarEntityId = 0;

        private GameState _gameState;

        [Inject]
        public void Construct(GameState gameState) => _gameState = gameState;

        private void Start()
        {
            var p = new PlayerId(playerId);
            var e = new GameEntityId(avatarEntityId);

            _gameState.PlayerAvatars.Set(p, e);
            _gameState.GetOrCreateEntity(e);
        }
    }
}