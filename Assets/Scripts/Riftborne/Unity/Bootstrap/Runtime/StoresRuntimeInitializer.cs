using Riftborne.Core.Stores;
using VContainer;

namespace Riftborne.Unity.Bootstrap.Runtime
{
    public sealed class StoresRuntimeInitializer : IRuntimeInitializer
    {
        public int Order => 200;

        public void Initialize(IContainerBuilder builder)
        {
            builder.Register<IMotorInputStore, MotorInputStore>(Lifetime.Singleton);
            builder.Register<IMotorStateStore, MotorStateStore>(Lifetime.Singleton);
            builder.Register<IActionIntentStore, ActionIntentStore>(Lifetime.Singleton);
            builder.Register<IAttackChargeStore, AttackChargeStore>(Lifetime.Singleton);
            builder.Register<IStatsStore, StatsStore>(Lifetime.Singleton);
            builder.Register<IStatsDeltaStore, StatsDeltaStore>(Lifetime.Singleton);        
            builder.Register<IStatsEffectStore, StatsEffectStore>(Lifetime.Singleton);
            builder.Register<IAttackCooldownStore, AttackCooldownStore>(Lifetime.Singleton);
        }
    }
}