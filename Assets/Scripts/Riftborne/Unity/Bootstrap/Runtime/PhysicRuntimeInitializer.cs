using Riftborne.App.Physics.Providers;
using Riftborne.App.Physics.Registry;
using Riftborne.Configs;
using Riftborne.Core.Gameplay.Physics.Providers;
using Riftborne.Core.Model;
using Riftborne.Core.Physics.Abstractions;
using Riftborne.Core.Physics.Motors;
using Riftborne.Core.TIme;
using Riftborne.Physics.Unity2D;
using VContainer;
using VContainer.Unity;

namespace Riftborne.Unity.Bootstrap.Runtime
{
    public sealed class PhysicRuntimeInitializer : IRuntimeInitializer
    {
        public int Order => 300;
        
        private readonly MotorConfigAsset _motorConfig;

        public PhysicRuntimeInitializer(MotorConfigAsset motorConfig)
        {
            _motorConfig = motorConfig;
        }


        public void Initialize(IContainerBuilder builder)
        {
            Physics2DScriptBootstrap.EnsureScriptMode();

            builder.Register(c =>
            {
                var config = c.Resolve<MotorConfigAsset>();
                var sim = c.Resolve<SimulationParameters>();
                return config.ToMotorParams(sim);
            }, Lifetime.Singleton);

            builder.Register<IBodyProvider<GameEntityId>, BodyRegistry>(Lifetime.Singleton);
            builder.Register<IPhysicsModifiersProvider, StatsPhysicsModifiersProvider>(Lifetime.Singleton);

            builder.Register<IPhysicsWorld, Unity2DPhysicsWorld>(Lifetime.Singleton);
            builder.Register<IGroundSensor, Unity2DGroundSensor>(Lifetime.Singleton);
            builder.Register<Unity2DWallSensor>(Lifetime.Singleton).As<IWallSensor>();
            builder.Register<ICharacterMotor, PlatformerCharacterMotor>(Lifetime.Singleton);
            builder.RegisterComponentInHierarchy<PhysicsBodyAuthoring>();
        }
    }
}