using Riftborne.Configs;
using Riftborne.Core.Factory;
using Riftborne.Core.Stats;
using VContainer;

namespace Riftborne.Unity.Bootstrap.Runtime
{
    public sealed class StatsRuntimeInitializer : IRuntimeInitializer
    {
        public int Order => 150;

        public void Initialize(IContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var cfg = c.Resolve<StatsConfigAsset>();
                return cfg.ToDefaults();
            }, Lifetime.Singleton);
            
            builder.Register<IStatsEffectCatalog, StatsEffectCatalog>(Lifetime.Singleton);
            builder.Register<IStatsEffectFactory, StatsEffectFactory>(Lifetime.Singleton);
        }
    }
}