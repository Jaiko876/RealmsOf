using Riftborne.Core.Physics.Abstractions;
using Riftborne.Physics;
using Riftborne.Physics.Unity2D;
using VContainer;
using VContainer.Unity;

namespace Riftborne.Unity.Bootstrap.Runtime
{
    public sealed class PhysicRuntimeInitializer : IRuntimeInitializer
    {
        public int Order => 100;
        
        public void Initialize(IContainerBuilder builder)
        {
            Physics2DScriptBootstrap.EnsureScriptMode();

            builder.Register<Unity2DPhysicsWorld>(Lifetime.Singleton).As<IPhysicsWorld>();
            builder.Register<Unity2DGroundSensor>(Lifetime.Singleton).As<IGroundSensor>();
            builder.Register<PlatformerCharacterMotor>(Lifetime.Singleton).As<ICharacterMotor>();
            builder.RegisterComponentInHierarchy<PhysicsBodyAuthoring>();
        }
    }
}