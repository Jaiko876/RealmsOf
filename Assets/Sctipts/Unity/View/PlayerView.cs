using UnityEngine;
using Game.Core.Model;
using Game.Core.Simulation;
using VContainer;

namespace Game.Unity.View
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField]
        private int playerId = 0;

        private ISimulation _simulation;

        [Inject]
        public void Construct(ISimulation simulation)
        {
            _simulation = simulation;
        }

        private void LateUpdate()
        {
            if (_simulation == null)
                return;

            PlayerId id = new PlayerId(playerId);
            PlayerState player = _simulation.State.GetOrCreatePlayer(id);

            transform.position = new Vector3(player.X, player.Y, transform.position.z);
        }
    }
}
