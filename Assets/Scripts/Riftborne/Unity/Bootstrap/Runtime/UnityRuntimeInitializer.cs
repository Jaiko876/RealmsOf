using Riftborne.Unity.Debugging;
using Riftborne.Unity.UI;
using Riftborne.Unity.View;
using Riftborne.Unity.View.Presenters;
using Riftborne.Unity.View.Presenters.Abstractions;
using VContainer;
using VContainer.Unity;

namespace Riftborne.Unity.Bootstrap.Runtime
{
    public sealed class UnityRuntimeInitializer : IRuntimeInitializer
    {
        public int Order => 1000;

        public void Initialize(IContainerBuilder builder)
        {
            builder.Register<IEntityAnimatorPresenter, EntityAnimatorPresenter>(Lifetime.Transient);
            builder.Register<IEntityTransformPresenter, EntityTransformPresenter>(Lifetime.Singleton);

            builder.RegisterComponentInHierarchy<PlayerStatsDebugLog>();
            // --- Scene components (в root, чтобы composer мог их найти) ---
            builder.RegisterComponentInHierarchy<EntityView>();
            builder.RegisterComponentInHierarchy<PlayerAvatarBinding>();
                
            builder.RegisterComponentInHierarchy<PlayerHudBarsPresenter>();
            builder.RegisterComponentInHierarchy<WorldStaggerBarView>();
        }
    }
}