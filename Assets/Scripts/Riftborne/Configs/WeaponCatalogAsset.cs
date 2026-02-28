using System;
using Riftborne.Core.Gameplay.Weapons.Model;
using UnityEngine;

namespace Riftborne.Configs
{
    [CreateAssetMenu(menuName = "Riftborne/Config/Weapons/Weapon Catalog", fileName = "WeaponCatalog")]
    public sealed class WeaponCatalogAsset : ScriptableObject
    {
        [Serializable]
        private struct WeaponEntry
        {
            public WeaponId Id;

            [Header("Light Hit (local)")]
            public float LightOffsetX;
            public float LightOffsetY;
            public float LightWidth;
            public float LightHeight;

            [Header("Heavy Hit (local)")]
            public float HeavyOffsetX;
            public float HeavyOffsetY;
            public float HeavyWidth;
            public float HeavyHeight;
        }

        [SerializeField] private WeaponEntry[] weapons;

        public WeaponDefinition[] Build()
        {
            if (weapons == null || weapons.Length == 0)
                return Array.Empty<WeaponDefinition>();

            var arr = new WeaponDefinition[weapons.Length];
            for (int i = 0; i < weapons.Length; i++)
            {
                var e = weapons[i];
                var light = new HitProfile(e.LightOffsetX, e.LightOffsetY, e.LightWidth, e.LightHeight);
                var heavy = new HitProfile(e.HeavyOffsetX, e.HeavyOffsetY, e.HeavyWidth, e.HeavyHeight);
                arr[i] = new WeaponDefinition(e.Id, light, heavy);
            }

            return arr;
        }

        private void OnValidate()
        {
            if (weapons == null) return;

            for (int i = 0; i < weapons.Length; i++)
            {
                weapons[i].LightWidth = Mathf.Max(0.01f, weapons[i].LightWidth);
                weapons[i].LightHeight = Mathf.Max(0.01f, weapons[i].LightHeight);
                weapons[i].HeavyWidth = Mathf.Max(0.01f, weapons[i].HeavyWidth);
                weapons[i].HeavyHeight = Mathf.Max(0.01f, weapons[i].HeavyHeight);
            }
        }
    }
}