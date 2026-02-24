using Riftborne.App.Random;
using Riftborne.Core.Random.Abstractions;
using VContainer;

namespace Riftborne.Unity.Bootstrap.Runtime
{
    public sealed class RandomRuntimeInitializer : IRuntimeInitializer
    {
        public int Order => 100;

        public void Initialize(IContainerBuilder builder)
        {
            builder.Register<IRandomFactory, XorShiftRandomFactory>(Lifetime.Singleton);
        }
    }
}