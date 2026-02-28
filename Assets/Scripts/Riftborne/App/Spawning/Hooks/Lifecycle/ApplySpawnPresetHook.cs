using System;
using Riftborne.Configs;
using Riftborne.Core.Gameplay.Weapons.Model;
using Riftborne.Core.Model;
using Riftborne.Core.Stores.Abstractions;

namespace Riftborne.App.Spawning.Hooks.Lifecycle
{
    public sealed class ApplySpawnPresetHook : OrderedEntityLifecycleHookBase
    {
        // Должен идти очень рано: это "инициализация симуляции".
        public override int Order => -1000;

        private readonly SpawnPresetAsset _presets;
        private readonly IEquippedWeaponStore _weapons;

        public ApplySpawnPresetHook(SpawnPresetAsset presets, IEquippedWeaponStore weapons)
        {
            _presets = presets != null ? presets : throw new ArgumentNullException(nameof(presets));
            _weapons = weapons ?? throw new ArgumentNullException(nameof(weapons));
        }

        public override void OnBeforeSpawn(GameEntityId id, string prefabKey, float x, float y)
        {
            // 1) если weapon уже выставлен (например, сейв/лут/скрипт) — не трогаем
            var current = _weapons.GetOrDefault(id, WeaponId.None);
            if (current != WeaponId.None)
                return;

            // 2) если для prefabKey есть пресет — применяем
            if (_presets.TryGet(prefabKey, out var defaultWeapon) && defaultWeapon != WeaponId.None)
            {
                _weapons.Set(id, defaultWeapon);
                return;
            }

            // 3) дефолт по платформе: кулаки
            _weapons.Set(id, WeaponId.Fists);
        }

        public override void OnAfterDespawn(GameEntityId id)
        {
            _weapons.Remove(id);
        }
    }
}