using System.Collections.Generic;
using Riftborne.Core.Gameplay.Combat.Model;
using Riftborne.Core.Model;
using Riftborne.Core.Stores.Abstractions;

namespace Riftborne.Core.Stores
{
    public sealed class CombatActionStore : ICombatActionStore
    {
        private readonly Dictionary<GameEntityId, CombatActionInstance> _map =
            new Dictionary<GameEntityId, CombatActionInstance>(128);

        public bool TryGet(GameEntityId id, out CombatActionInstance action) => _map.TryGetValue(id, out action);

        public void Set(GameEntityId id, CombatActionInstance action)
        {
            if (action.Type == CombatActionType.None)
            {
                _map.Remove(id);
                return;
            }

            _map[id] = action;
        }

        public void Remove(GameEntityId id) => _map.Remove(id);
        public void Clear() => _map.Clear();
    }
}