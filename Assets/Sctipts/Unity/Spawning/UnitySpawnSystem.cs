using Game.Core.Spawning;
using Game.Core.Model;
using UnityEngine;

namespace Game.Unity.Spawning
{
    public sealed class UnitySpawnSystem : MonoBehaviour, ISpawnSystem
    {
        [SerializeField] private Transform _playerTransform;

        public void Spawn(in SpawnRequest request)
        {
            if (request.Type == "Player")
            {
                _playerTransform.position =
                    new Vector3(request.X + 0.5f, request.Y + 0.5f, 0f);
            }
        }
    }
}
