using Riftborne.App.Combat.Providers;
using Riftborne.App.Combat.Providers.Abstractions;
using Riftborne.Core.Gameplay.Combat.Rules;
using Riftborne.Core.Gameplay.Combat.Rules.Abstractions;
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
        }
    }
}