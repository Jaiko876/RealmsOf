using Riftborne.Core.Input;
using VContainer;

namespace Riftborne.Unity.Bootstrap.Runtime
{
    public sealed class StoresRuntimeInitializer : IRuntimeInitializer
    {
        public int Order => 200;

        public void Initialize(IContainerBuilder builder)
        {
            builder.Register<IMotorInputStore, MotorInputStore>(Lifetime.Singleton);
        }
    }
}