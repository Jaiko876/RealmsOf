using Riftborne.Core.Spawning;
using UnityEngine;

namespace Riftborne.Unity.Spawning
{
    public sealed class UnitySpawnSystem : MonoBehaviour, ISpawnSystem
    {
        [Header("Player")]
        [SerializeField] private Transform _playerTransform;

        [Header("Objects")]
        [SerializeField] private GameObject _spawnerPrefab;

        public void Spawn(in SpawnRequest request)
        {
            if (request.Type == "Player")
            {
                if (_playerTransform == null) return;
                _playerTransform.position = new Vector3(request.X + 0.5f, request.Y + 0.5f, 0f);
                return;
            }

            if (request.Type == "Spawner")
            {
                if (_spawnerPrefab == null) return;
                Instantiate(_spawnerPrefab, new Vector3(request.X + 0.5f, request.Y + 0.5f, 0f), Quaternion.identity);
                return;
            }
        }
    }
}
