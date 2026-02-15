using System.Collections.Generic;
using Game.Core.Model;
using Game.Core.Physics.Abstractions;

namespace Game.Physics.Unity2D
{

    /// <summary>
    /// Opt-in registry: only registered players are driven by physics.
    /// </summary>
    public sealed class PlayerBodyRegistry : IPlayerBodyProvider
    {
        private readonly Dictionary<PlayerId, IPhysicsBody> _map = new();

        public void Register(PlayerId id, IPhysicsBody body) => _map[id] = body;
        public void Unregister(PlayerId id) => _map.Remove(id);

        public bool TryGet(PlayerId id, out IPhysicsBody body) => _map.TryGetValue(id, out body);
    }
}
