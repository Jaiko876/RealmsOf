// Assets/Scripts/Riftborne/Unity/Bootstrap/Runtime/CombatRuntimeInitializer.cs

using Riftborne.App.Combat;
using Riftborne.App.Combat.Abstractions;
using Riftborne.App.Combat.Providers;
using Riftborne.App.Combat.Providers.Abstractions;
using Riftborne.Core.Gameplay.Combat.Abstractions;
using Riftborne.Core.Gameplay.Combat.Rules;
using Riftborne.Core.Gameplay.Combat.Rules.Abstractions;
using Riftborne.Unity.Physics.Unity2D;
using VContainer;

namespace Riftborne.Unity.Bootstrap.Runtime
{
    public sealed class CombatRuntimeInitializer : IRuntimeInitializer
    {
        public int Order => 900;

        public void Initialize(IContainerBuilder builder)
        {
            builder.Register<IAttackInputRules, AttackInputRules>(Lifetime.Singleton);
            builder.Register<ICombatSpeedProvider, StatsCombatSpeedProvider>(Lifetime.Singleton);

            builder.Register<ICombatActionStarter, CombatActionStarter>(Lifetime.Singleton);

            builder.Register<ICombatHitRules, BasicCombatHitRules>(Lifetime.Singleton);
            builder.Register<ICombatHitQuery, Unity2DCombatHitQuery>(Lifetime.Singleton);
        }
    }
}