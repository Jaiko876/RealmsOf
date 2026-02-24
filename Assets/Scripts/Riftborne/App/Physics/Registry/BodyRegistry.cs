using System.Collections.Generic;
using Riftborne.Core.Model;
using Riftborne.Core.Physics.Abstractions;

namespace Riftborne.App.Physics.Registry
{
    public sealed class BodyRegistry : IBodyProvider<GameEntityId>
    {
        private readonly Dictionary<GameEntityId, IPhysicsBody> _map = new();

        private readonly List<GameEntityId> _ids = new();
        private bool _dirty = true;

        public bool TryGet(GameEntityId id, out IPhysicsBody body)
        {
            return _map.TryGetValue(id, out body);
        }

        public void Register(GameEntityId id, IPhysicsBody body)
        {
            _map[id] = body;
            _dirty = true;
        }

        public void Unregister(GameEntityId id)
        {
            if (_map.Remove(id))
                _dirty = true;
        }

        public IEnumerable<GameEntityId> EnumerateIds()
        {
            if (_dirty)
            {
                _ids.Clear();
                _ids.AddRange(_map.Keys);
                _dirty = false;
            }

            return _ids;
        }
    }
}