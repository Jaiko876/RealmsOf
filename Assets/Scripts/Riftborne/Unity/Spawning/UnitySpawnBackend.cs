using System;
using System.Collections.Generic;
using Riftborne.App.Spawning.Abstractions;
using Riftborne.Core.Entities;
using Riftborne.Core.Model;
using Riftborne.Unity.Entities;
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

        private IObjectResolver _resolver;

        private readonly Dictionary<string, GameObject> _catalog =
            new Dictionary<string, GameObject>(StringComparer.Ordinal);

        private readonly Dictionary<int, GameObject> _alive =
            new Dictionary<int, GameObject>();

        private Transform _stagingRoot;

        [Inject]
        public void Construct(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        private void Awake()
        {
            BuildCatalog();
            EnsureStagingRoot();
        }

        private void BuildCatalog()
        {
            _catalog.Clear();

            if (prefabs == null)
                return;

            for (int i = 0; i < prefabs.Length; i++)
            {
                var e = prefabs[i];
                if (string.IsNullOrEmpty(e.Key) || e.Prefab == null)
                    continue;

                _catalog[e.Key] = e.Prefab;
            }
        }

        private void EnsureStagingRoot()
        {
            if (_stagingRoot != null)
                return;

            var go = new GameObject("__SpawnStaging");
            go.hideFlags = HideFlags.HideInHierarchy;
            go.SetActive(false);
            _stagingRoot = go.transform;
        }

        public void Spawn(GameEntityId id, string prefabKey, float x, float y)
        {
            if (_alive.ContainsKey(id.Value))
                throw new InvalidOperationException("Entity already spawned: " + id.Value);

            if (!_catalog.TryGetValue(prefabKey, out var prefab) || prefab == null)
                throw new InvalidOperationException("Unknown prefabKey: " + prefabKey);

            EnsureStagingRoot();

            // 1) Instantiate under inactive staging root => NO OnEnable/Start yet
            var instance = Instantiate(prefab, _stagingRoot, false);
            instance.name = prefab.name + "_" + id.Value;

            // Make sure activeSelf is false while we wire identity + DI
            instance.SetActive(false);

            // 2) Position
            instance.transform.position = new Vector3(x, y, 0f);
            instance.transform.rotation = Quaternion.identity;

            // 3) Identity-first: assign entityId (and optionally owner)
            ApplyEntityId(instance, id);

            // 4) DI injection while inactive
            if (_resolver != null)
                _resolver.InjectGameObject(instance);

            // 5) Move to world + enable (OnEnable/Start will run now)
            instance.transform.SetParent(null, true);
            instance.SetActive(true);

            _alive[id.Value] = instance;
        }

        public void Despawn(GameEntityId id)
        {
            if (_alive.TryGetValue(id.Value, out var go) && go != null)
            {
                _alive.Remove(id.Value);
                Destroy(go);
            }
        }

        private static void ApplyEntityId(GameObject root, GameEntityId id)
        {
            var identity = root.GetComponent<EntityIdentityAuthoring>();
            if (identity != null)
            {
                identity.AssignEntityId(id);
                return;
            }

            // Fallback: broadcast to receivers (if prefab doesn't have identity component yet)
            var behaviours = root.GetComponentsInChildren<MonoBehaviour>(true);
            for (int i = 0; i < behaviours.Length; i++)
            {
                var b = behaviours[i];
                var rx = b as IGameEntityIdReceiver;
                if (rx != null)
                    rx.SetEntityId(id);
            }
        }
    }
}