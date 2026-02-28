using Riftborne.App.Weapons.Catalog;
using Riftborne.Configs;
using Riftborne.Core.Gameplay.Weapons.Abstractions;
using VContainer;

namespace Riftborne.Unity.Bootstrap.Runtime
{
    public sealed class WeaponsRuntimeInitializer : IRuntimeInitializer
    {
        public int Order => 850;

        public void Initialize(IContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var asset = c.Resolve<WeaponCatalogAsset>();
                return asset.Build();
            }, Lifetime.Singleton);

            builder.Register<IWeaponCatalog, WeaponCatalog>(Lifetime.Singleton);        }
    }
}