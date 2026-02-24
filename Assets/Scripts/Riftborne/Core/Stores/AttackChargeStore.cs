using System.Collections.Generic;
using Riftborne.Core.Model;
using Riftborne.Core.Stores.Abstractions;

namespace Riftborne.Core.Stores
{
    public sealed class AttackChargeStore : IAttackChargeStore
    {
        private struct Entry
        {
            public bool Charging;
            public float Charge01;
        }

        private readonly Dictionary<GameEntityId, Entry> _map = new();

        public void Set(GameEntityId id, bool charging, float charge01)
        {
            _map[id] = new Entry { Charging = charging, Charge01 = charge01 };
        }

        public bool TryGet(GameEntityId id, out bool charging, out float charge01)
        {
            if (_map.TryGetValue(id, out var e))
            {
                charging = e.Charging;
                charge01 = e.Charge01;
                return true;
            }

            charging = false;
            charge01 = 0f;
            return false;
        }

        public void Remove(GameEntityId id) => _map.Remove(id);
        public void Clear() => _map.Clear();
    }
}