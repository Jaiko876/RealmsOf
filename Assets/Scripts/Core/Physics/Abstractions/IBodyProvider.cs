namespace Game.Core.Physics.Abstractions
{
    public interface IBodyProvider<TId>
    {
        bool TryGet(TId id, out IPhysicsBody body);
        void Register(TId id, IPhysicsBody body);
        void Unregister(TId id);
    }
}
