using Riftborne.Core.Simulation;
using VContainer;
using VContainer.Unity;

namespace Riftborne.Unity.Bootstrap.Runtime
{
    public sealed class SimulationRuntimeInitializer : IRuntimeInitializer
    {
        public int Order => 400;

        public void Initialize(IContainerBuilder builder)
        {
            builder.Register<ISimulation, Simulation>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GameLoop>();
        }
    }
}