using System.Collections.Generic;

namespace Game.Core.Model
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
    }
}
