using System.Collections.Generic;

namespace Riftborne.Core.Model
{
    public sealed class PlayerAvatarMap
    {
        private readonly Dictionary<PlayerId, GameEntityId> _map = new Dictionary<PlayerId, GameEntityId>();

        public bool TryGet(PlayerId playerId, out GameEntityId entityId)
        {
            return _map.TryGetValue(playerId, out entityId);
        }

        public void Set(PlayerId playerId, GameEntityId entityId)
        {
            _map[playerId] = entityId;
        }

        public void Remove(PlayerId playerId)
        {
            _map.Remove(playerId);
        }
        
        public void RemoveByEntity(GameEntityId entityId)
        {
            PlayerId found = default;
            bool has = false;

            foreach (var kv in _map)
            {
                if (kv.Value.Equals(entityId))
                {
                    found = kv.Key;
                    has = true;
                    break;
                }
            }

            if (has)
                _map.Remove(found);
        }
    }
}
