using Riftborne.Core.Animation.Abstractions;
using Riftborne.Core.Animation.Provider;
using VContainer;

namespace Riftborne.Unity.Bootstrap.Runtime
{
    public sealed class AnimationsRuntimeInitializer : IRuntimeInitializer
    {
        public int Order => 700;

        public void Initialize(IContainerBuilder builder)
        {
            // Конкретные обработчики (без регистрации одного и того же ICommandHandler<> два раза)
            builder.Register<IAnimationModifiersProvider, StatsAnimationModifiersProvider>(Lifetime.Singleton);
        }
    }
}