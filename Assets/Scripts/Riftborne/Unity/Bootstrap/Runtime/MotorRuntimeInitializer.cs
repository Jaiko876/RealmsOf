using Riftborne.Configs;
using Riftborne.Core.Model;
using Riftborne.Core.Physics.Abstractions;
using Riftborne.Core.Physics.Model;
using Riftborne.Core.Physics.Registry;
using VContainer;

namespace Riftborne.Unity.Bootstrap.Runtime
{
    public sealed class MotorRuntimeInitializer : IRuntimeInitializer
    {
        private readonly MotorConfigAsset _motorConfig;

        public MotorRuntimeInitializer(MotorConfigAsset motorConfig)
        {
            _motorConfig = motorConfig;
        }

        public int Order => 200;

        public void Initialize(IContainerBuilder builder)
        {
            MotorParams motorParams = _motorConfig.ToMotorParams();
            builder.RegisterInstance(motorParams);

            builder.Register<BodyRegistry>(Lifetime.Singleton)
                .As<IBodyProvider<GameEntityId>>();
        }
    }
}