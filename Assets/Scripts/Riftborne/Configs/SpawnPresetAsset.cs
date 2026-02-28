using System;
using Riftborne.Core.Gameplay.Weapons.Model;
using UnityEngine;

namespace Riftborne.Configs
{
    [CreateAssetMenu(menuName = "Riftborne/Config/Spawning/Spawn Presets", fileName = "SpawnPresets")]
    public sealed class SpawnPresetAsset : ScriptableObject
    {
        [Serializable]
        private struct Entry
        {
            public string PrefabKey;

            [Header("Weapons")]
            public WeaponId DefaultWeapon;
        }

        [SerializeField] private Entry[] entries;

        public bool TryGet(string prefabKey, out WeaponId defaultWeapon)
        {
            defaultWeapon = WeaponId.None;

            if (string.IsNullOrWhiteSpace(prefabKey))
                return false;

            if (entries == null)
                return false;

            for (int i = 0; i < entries.Length; i++)
            {
                var e = entries[i];
                if (!string.Equals(e.PrefabKey, prefabKey, StringComparison.Ordinal))
                    continue;

                defaultWeapon = e.DefaultWeapon;
                return true;
            }

            return false;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (entries == null) return;

            for (int i = 0; i < entries.Length; i++)
            {
                var key = entries[i].PrefabKey;

                if (string.IsNullOrWhiteSpace(key))
                {
                    Debug.LogError("SpawnPresetAsset has empty PrefabKey at index " + i, this);
                    return;
                }

                for (int j = i + 1; j < entries.Length; j++)
                {
                    if (string.Equals(entries[j].PrefabKey, key, StringComparison.Ordinal))
                    {
                        Debug.LogError("SpawnPresetAsset has duplicate PrefabKey: " + key, this);
                        return;
                    }
                }
            }
        }
#endif
    }
}