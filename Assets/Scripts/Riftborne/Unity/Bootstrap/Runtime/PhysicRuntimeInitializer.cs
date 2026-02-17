using Riftborne.Configs;
using Riftborne.Core.Model;
using Riftborne.Core.Physics.Abstractions;
using Riftborne.Core.Physics.Model;
using Riftborne.Core.Physics.Registry;
using Riftborne.Physics;
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
            
            MotorParams motorParams = _motorConfig.ToMotorParams();
            builder.RegisterInstance(motorParams);

            builder.Register<IBodyProvider<GameEntityId>, BodyRegistry>(Lifetime.Singleton);

            builder.Register<IPhysicsWorld, Unity2DPhysicsWorld>(Lifetime.Singleton);
            builder.Register<IGroundSensor, Unity2DGroundSensor>(Lifetime.Singleton);
            builder.Register<ICharacterMotor, PlatformerCharacterMotor>(Lifetime.Singleton);
            builder.RegisterComponentInHierarchy<PhysicsBodyAuthoring>();
        }
    }
}