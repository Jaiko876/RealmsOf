using Game.Domain.Model;
using Game.Simulation;
using UnityEngine;
using VContainer;

namespace Game.Presentation.View
{

    public sealed class PlayerView : MonoBehaviour
    {
        [SerializeField] private int playerId = 0;

        private ISimulation _simulation = null!;

        [Inject]
        public void Construct(ISimulation simulation) => _simulation = simulation;

        private void LateUpdate()
        {
            var id = new PlayerId(playerId);
            var player = _simulation.State.GetOrCreatePlayer(id);

            transform.position = new Vector3(player.X, player.Y, transform.position.z);
        }
    }
}