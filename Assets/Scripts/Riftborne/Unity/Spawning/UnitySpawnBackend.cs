using System;
using System.Collections.Generic;
using Riftborne.Core.Model;
using Riftborne.Core.Spawning;
using Riftborne.Physics.Unity2D;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Riftborne.Unity.Spawning
{
    public sealed class UnitySpawnBackend : MonoBehaviour, ISpawnBackend
    {
        [Serializable]
        private struct Entry
        {
            public string Key;
            public GameObject Prefab;
        }

        [SerializeField] private Entry[] prefabs;
        private readonly IObjectResolver _resolver;

        private readonly Dictionary<string, GameObject> _catalog = new Dictionary<string, GameObject>(StringComparer.Ordinal);
        private readonly Dictionary<int, GameObject> _alive = new Dictionary<int, GameObject>();

        public UnitySpawnBackend(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        private void Awake()
        {
            _catalog.Clear();
            if (prefabs == null) return;

            for (int i = 0; i < prefabs.Length; i++)
            {
                var e = prefabs[i];
                if (string.IsNullOrEmpty(e.Key) || e.Prefab == null) continue;
                _catalog[e.Key] = e.Prefab;
            }
        }

        public void Spawn(GameEntityId id, string prefabKey, float x, float y)
        {
            if (_alive.ContainsKey(id.Value))
                throw new InvalidOperationException("Entity already spawned: " + id.Value);

            if (!_catalog.TryGetValue(prefabKey, out var prefab) || prefab == null)
                throw new InvalidOperationException("Unknown prefabKey: " + prefabKey);

            var go = Instantiate(prefab, new Vector3(x, y, 0f), Quaternion.identity);
            
            _resolver.InjectGameObject(go);

            // важно: проставить id до регистрации body
            var body = go.GetComponentInChildren<PhysicsBodyAuthoring>(true);
            if (body != null)
                body.SetEntityId(id);

            _alive[id.Value] = go;
        }

        public void Despawn(GameEntityId id)
        {
            if (_alive.TryGetValue(id.Value, out var go) && go != null)
            {
                _alive.Remove(id.Value);
                Destroy(go);
            }
        }
    }
}