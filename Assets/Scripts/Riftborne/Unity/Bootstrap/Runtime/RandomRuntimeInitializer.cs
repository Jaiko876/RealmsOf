using Riftborne.Core.Factory;
using VContainer;

namespace Riftborne.Unity.Bootstrap.Runtime
{
    public sealed class RandomRuntimeInitializer : IRuntimeInitializer
    {
        public int Order => 500;

        public void Initialize(IContainerBuilder builder)
        {
            builder.Register<XorShiftRandomFactory>(Lifetime.Singleton).As<IRandomFactory>();
        }
    }
}