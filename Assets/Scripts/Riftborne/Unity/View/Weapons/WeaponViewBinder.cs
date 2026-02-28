using Riftborne.Configs;
using Riftborne.Core.Entities;
using Riftborne.Core.Gameplay.Weapons.Model;
using Riftborne.Core.Model;
using Riftborne.Core.Stores.Abstractions;
using UnityEngine;
using VContainer;

namespace Riftborne.Unity.View.Weapons
{
    public sealed class WeaponViewBinder : MonoBehaviour, IGameEntityIdReceiver
    {
        [Header("Scene fallback (debug only)")]
        [SerializeField] private int sceneEntityId = 0;

        [SerializeField] private Transform weaponSlot;

        private GameEntityId _id;
        private bool _hasId;

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

        public void SetEntityId(GameEntityId id)
        {
            _id = id;
            _hasId = true;
            sceneEntityId = id.Value;
        }

        private void OnEnable()
        {
            if (!_hasId)
            {
                _id = new GameEntityId(sceneEntityId);
                _hasId = true;
            }
        }

        private void LateUpdate()
        {
            if (!_hasId || _weapons == null || _views == null || weaponSlot == null)
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