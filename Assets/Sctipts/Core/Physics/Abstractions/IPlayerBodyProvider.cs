using Game.Core.Model;

namespace Game.Core.Physics.Abstractions
{

    /// <summary>
    /// Maps PlayerId -> physics body. Opt-in: if a player has no body, they are "non-physical".
    /// </summary>
    public interface IPlayerBodyProvider
    {
        bool TryGet(PlayerId id, out IPhysicsBody body);
        void Register(PlayerId id, IPhysicsBody body);
        void Unregister(PlayerId id);
    }
}
