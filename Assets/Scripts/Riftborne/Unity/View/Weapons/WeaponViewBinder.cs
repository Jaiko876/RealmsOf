using Riftborne.Configs;
using Riftborne.Core.Gameplay.Weapons.Model;
using Riftborne.Core.Model;
using Riftborne.Core.Stores.Abstractions;
using UnityEngine;
using VContainer;

namespace Riftborne.Unity.View.Weapons
{
    public sealed class WeaponViewBinder : MonoBehaviour
    {
        [SerializeField] private int controlledEntityId;
        [SerializeField] private Transform weaponSlot;

        private GameEntityId _id;

        private IEquippedWeaponStore _weapons;
        private WeaponViewCatalogAsset _views;

        private WeaponId _shown = WeaponId.None;
        private GameObject _instance;

        [Inject]
        public void Construct(IEquippedWeaponStore weapons, WeaponViewCatalogAsset views)
        {
            _weapons = weapons;
            _views = views;
        }

        private void Awake()
        {
            _id = new GameEntityId(controlledEntityId);
        }

        private void LateUpdate()
        {
            if (_weapons == null || _views == null || weaponSlot == null)
                return;

            var desired = _weapons.GetOrDefault(_id, WeaponId.Fists);
            if (desired == _shown)
                return;

            Replace(desired);
        }

        private void Replace(WeaponId weaponId)
        {
            DestroyCurrent();

            _shown = weaponId;

            if (weaponId == WeaponId.None || weaponId == WeaponId.Fists)
                return;

            if (!_views.TryGetPrefab(weaponId, out var prefab))
                return;

            _instance = Instantiate(prefab, weaponSlot, false);
        }

        private void DestroyCurrent()
        {
            if (_instance == null) return;
            Destroy(_instance);
            _instance = null;
        }

        private void OnDestroy()
        {
            DestroyCurrent();
        }
    }
}