namespace Riftborne.App.Spawning.Hooks.Lifecycle
{
    public interface IOrderedEntityLifecycleHook : IEntityLifecycleHook
    {
        int Order { get; }
    }

    public abstract class OrderedEntityLifecycleHookBase : EntityLifecycleHookBase, IOrderedEntityLifecycleHook
    {
        public abstract int Order { get; }
    }
}