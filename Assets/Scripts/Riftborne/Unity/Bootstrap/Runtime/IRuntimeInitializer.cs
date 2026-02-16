using VContainer;

namespace Riftborne.Unity.Bootstrap.Runtime
{
    public interface IRuntimeInitializer
    {
        int Order { get; }
        void  Initialize(IContainerBuilder builder);
    }
}