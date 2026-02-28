using System;
using Riftborne.Core.Gameplay.Weapons.Model;
using UnityEngine;

namespace Riftborne.Configs
{
    [CreateAssetMenu(menuName = "Riftborne/Config/Weapons/Weapon View Catalog", fileName = "WeaponViewCatalog")]
    public sealed class WeaponViewCatalogAsset : ScriptableObject
    {
        [Serializable]
        private struct Entry
        {
            public WeaponId Id;
            public GameObject Prefab;
        }

        [SerializeField] private Entry[] entries;

        public bool TryGetPrefab(WeaponId id, out GameObject prefab)
        {
            prefab = null;
            if (entries == null) return false;

            for (int i = 0; i < entries.Length; i++)
            {
                var e = entries[i];
                if (e.Id != id) continue;

                prefab = e.Prefab;
                return prefab != null;
            }

            return false;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (entries == null) return;

            for (int i = 0; i < entries.Length; i++)
            {
                var id = entries[i].Id;
                for (int j = i + 1; j < entries.Length; j++)
                {
                    if (entries[j].Id == id)
                    {
                        Debug.LogError("WeaponViewCatalogAsset has duplicate WeaponId: " + id, this);
                        return;
                    }
                }
            }
        }
#endif
    }
}