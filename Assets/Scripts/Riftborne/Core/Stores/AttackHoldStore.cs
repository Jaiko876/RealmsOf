using System.Collections.Generic;
using Riftborne.Core.Model;
using Riftborne.Core.Stores.Abstractions;

namespace Riftborne.Core.Stores
{
    public sealed class AttackHoldStore : IAttackHoldStore
    {
        private struct Entry
        {
            public bool PrevHeld;
            public int HeldTicks;
        }

        private readonly Dictionary<GameEntityId, Entry> _map = new Dictionary<GameEntityId, Entry>();

        public bool TryGet(GameEntityId id, out bool prevHeld, out int heldTicks)
        {
            if (_map.TryGetValue(id, out var e))
            {
                prevHeld = e.PrevHeld;
                heldTicks = e.HeldTicks;
                return true;
            }

            prevHeld = false;
            heldTicks = 0;
            return false;
        }

        public void Set(GameEntityId id, bool prevHeld, int heldTicks)
        {
            if (heldTicks < 0) heldTicks = 0;
            _map[id] = new Entry { PrevHeld = prevHeld, HeldTicks = heldTicks };
        }

        public void Remove(GameEntityId id) => _map.Remove(id);
        public void Clear() => _map.Clear();
    }
}