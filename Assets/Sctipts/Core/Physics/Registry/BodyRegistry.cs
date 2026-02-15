using System.Collections.Generic;
using Game.Core.Model;
using Game.Core.Physics.Abstractions;

namespace Game.Physics.Registry
{
    public sealed class BodyRegistry : IBodyProvider<GameEntityId>
    {
        private readonly Dictionary<GameEntityId, IPhysicsBody> _map =
            new Dictionary<GameEntityId, IPhysicsBody>();

        public bool TryGet(GameEntityId id, out IPhysicsBody body)
        {
            return _map.TryGetValue(id, out body);
        }

        public void Register(GameEntityId id, IPhysicsBody body)
        {
            _map[id] = body;
        }

        public void Unregister(GameEntityId id)
        {
            _map.Remove(id);
        }
    }
}
