using System.Collections.Generic;
using Riftborne.Core.Model;
using Riftborne.Core.Stores.Abstractions;

namespace Riftborne.Core.Stores
{
    public sealed class BlockStateStore : IBlockStateStore
    {
        private readonly Dictionary<GameEntityId, bool> _map = new Dictionary<GameEntityId, bool>(128);

        public bool IsBlocking(GameEntityId id)
            => _map.TryGetValue(id, out var v) && v;

        public void SetBlocking(GameEntityId id, bool blocking)
        {
            if (!blocking) _map.Remove(id);
            else _map[id] = true;
        }

        public void Remove(GameEntityId id) => _map.Remove(id);
        public void Clear() => _map.Clear();
    }
}