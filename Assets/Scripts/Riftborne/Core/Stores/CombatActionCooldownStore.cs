using System.Collections.Generic;
using Riftborne.Core.Gameplay.Combat.Model;
using Riftborne.Core.Model;
using Riftborne.Core.Stores.Abstractions;

namespace Riftborne.Core.Stores
{
    public sealed class CombatActionCooldownStore : ICombatActionCooldownStore
    {
        private struct Entry
        {
            public int NextParryTick;
            public int NextDodgeTick;
        }

        private readonly Dictionary<GameEntityId, Entry> _map = new Dictionary<GameEntityId, Entry>(128);

        public bool CanUse(GameEntityId id, CombatActionType action, int tick)
        {
            if (!_map.TryGetValue(id, out var e))
                return true;

            switch (action)
            {
                case CombatActionType.Parry:
                    return tick >= e.NextParryTick;

                case CombatActionType.Dodge:
                    return tick >= e.NextDodgeTick;

                default:
                    return true; // Attack cooldown is handled elsewhere
            }
        }

        public void Consume(GameEntityId id, CombatActionType action, int tick, int cooldownTicks)
        {
            if (cooldownTicks < 0) cooldownTicks = 0;

            _map.TryGetValue(id, out var e);

            switch (action)
            {
                case CombatActionType.Parry:
                    e.NextParryTick = tick + cooldownTicks;
                    _map[id] = e;
                    break;

                case CombatActionType.Dodge:
                    e.NextDodgeTick = tick + cooldownTicks;
                    _map[id] = e;
                    break;
            }
        }

        public void Remove(GameEntityId id) => _map.Remove(id);
        public void Clear() => _map.Clear();
    }
}