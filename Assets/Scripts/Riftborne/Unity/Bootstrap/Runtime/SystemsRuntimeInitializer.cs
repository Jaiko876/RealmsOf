using Riftborne.App.Animation.Systems;
using Riftborne.App.Combat.Systems;
using Riftborne.App.Simulation.Pipeline;
using Riftborne.Core.Systems.PostPhysicsTickSystems;
using Riftborne.Core.Systems.PrePhysicsTickSystems;
using VContainer;

namespace Riftborne.Unity.Bootstrap.Runtime
{
    public sealed class SystemsRuntimeInitializer : IRuntimeInitializer
    {
        public int Order => 500;

        public void Initialize(IContainerBuilder builder)
        {
            builder.Register<MotorPrePhysicsTickSystem>(Lifetime.Singleton);
            builder.Register<PostPhysicsStateSyncSystem>(Lifetime.Singleton);
            builder.Register<AnimationStatePostPhysicsSystem>(Lifetime.Singleton);
            builder.Register<StatsInitPostPhysicsSystem>(Lifetime.Singleton);
            builder.Register<StatsRegenPostPhysicsSystem>(Lifetime.Singleton);
            builder.Register<StatsApplyDeltasPostPhysicsSystem>(Lifetime.Singleton);
            builder.Register<StatsEffectsTickPostPhysicsSystem>(Lifetime.Singleton);
            builder.Register<StatsRebuildModifiersPostPhysicsSystem>(Lifetime.Singleton);
            builder.Register<CombatActionsPostPhysicsSystem>(Lifetime.Singleton);

            builder.Register<ITickPipeline, TickPipeline>(Lifetime.Singleton);
        }
    }
}