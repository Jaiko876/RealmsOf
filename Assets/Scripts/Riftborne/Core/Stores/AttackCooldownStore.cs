using System.Collections.Generic;
using Riftborne.Core.Model;

namespace Riftborne.Core.Stores
{
    public sealed class AttackCooldownStore : IAttackCooldownStore
    {
        private readonly Dictionary<GameEntityId, int> _nextAllowedTick = new Dictionary<GameEntityId, int>();

        public bool CanAttack(GameEntityId id, int tick)
        {
            if (_nextAllowedTick.TryGetValue(id, out var next))
                return tick >= next;

            return true;
        }

        public void ConsumeAttack(GameEntityId id, int tick, int cooldownTicks)
        {
            if (cooldownTicks < 0) cooldownTicks = 0;
            _nextAllowedTick[id] = tick + cooldownTicks;
        }

        public void Remove(GameEntityId id) => _nextAllowedTick.Remove(id);
        public void Clear() => _nextAllowedTick.Clear();
    }
}