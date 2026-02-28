using Riftborne.App.Spawning.Hooks.Lifecycle;
using Riftborne.App.Weapons.Catalog;
using Riftborne.Configs;
using Riftborne.Core.Gameplay.Weapons.Abstractions;
using Riftborne.Core.Stores.Abstractions;
using VContainer;

namespace Riftborne.Unity.Bootstrap.Runtime
{
    public sealed class WeaponsRuntimeInitializer : IRuntimeInitializer
    {
        public int Order => 850;

        public void Initialize(IContainerBuilder builder)
        {
            builder.Register<IEntityLifecycleHook>(c =>
                    new ApplySpawnPresetHook(
                        c.Resolve<SpawnPresetAsset>(),
                        c.Resolve<IEquippedWeaponStore>()),
                Lifetime.Singleton);

            builder.Register<IWeaponCatalog, WeaponCatalog>(Lifetime.Singleton);
        }
    }
}