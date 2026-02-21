using System.Collections.Generic;
using Riftborne.Core.Model;
using Riftborne.Core.Model.Animation;

namespace Riftborne.Core.Stores
{
    public sealed class ActionIntentStore : IActionIntentStore
    {
        private readonly Dictionary<GameEntityId, ActionState> _map = new();

        public void Set(GameEntityId id, ActionState action)
        {
            if (action == ActionState.None) return;
            _map[id] = action;
        }

        public bool TryConsume(GameEntityId id, out ActionState action)
        {
            if (_map.TryGetValue(id, out action))
            {
                _map.Remove(id);
                return true;
            }

            action = ActionState.None;
            return false;
        }
        
        public void Remove(GameEntityId id) => _map.Remove(id);

        public void Clear() => _map.Clear();
    }
}