using Riftborne.Unity.Debugging;
using Riftborne.Unity.UI;
using Riftborne.Unity.View.Presenters;
using Riftborne.Unity.View.Presenters.Abstractions;
using Riftborne.Unity.View.Registry;
using VContainer;
using VContainer.Unity;

namespace Riftborne.Unity.Bootstrap.Runtime
{
    public sealed class UnityRuntimeInitializer : IRuntimeInitializer
    {
        public int Order => 1000;

        public void Initialize(IContainerBuilder builder)
        {
            // presenters / registries
            builder.Register<IEntityAnimatorPresenter, EntityAnimatorPresenter>(Lifetime.Transient);
            builder.Register<IEntityTransformPresenter, EntityTransformPresenter>(Lifetime.Singleton);

            builder.Register<IEntityViewRegistry, EntityViewRegistry>(Lifetime.Singleton);

            // scene-level only
            builder.RegisterComponentInHierarchy<PlayerHudBarsPresenter>();
            builder.RegisterComponentInHierarchy<PlayerStatsDebugLog>();

            // local-player camera binder (scene-level)
            builder.RegisterComponentInHierarchy<LocalPlayerCameraBinder>();
        }
    }
}