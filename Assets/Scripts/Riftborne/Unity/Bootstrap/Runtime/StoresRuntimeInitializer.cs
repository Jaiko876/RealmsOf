using Riftborne.Core.Stores;
using Riftborne.Core.Stores.Abstractions;
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
            builder.Register<IAttackChargeStore, AttackChargeStore>(Lifetime.Singleton);
            builder.Register<IStatsStore, StatsStore>(Lifetime.Singleton);
            builder.Register<IStatsDeltaStore, StatsDeltaStore>(Lifetime.Singleton);        
            builder.Register<IStatsEffectStore, StatsEffectStore>(Lifetime.Singleton);
            builder.Register<IAttackCooldownStore, AttackCooldownStore>(Lifetime.Singleton);
            builder.Register<IActionEventStore, ActionEventStore>(Lifetime.Singleton);
            builder.Register<IAttackHoldStore, AttackHoldStore>(Lifetime.Singleton);
            builder.Register<IEquippedWeaponStore, EquippedWeaponStore>(Lifetime.Singleton);
            builder.Register<ICombatActionStore, CombatActionStore>(Lifetime.Singleton);
        }
    }
}