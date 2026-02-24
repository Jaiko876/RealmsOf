using System.Collections.Generic;
using Riftborne.Core.Model;
using Riftborne.Core.Model.Animation;

namespace Riftborne.Core.Stores
{
    public sealed class ActionTimingStore : IActionTimingStore
    {
        private struct Entry
        {
            public ActionState Action;
            public int DurationTicks;
        }

        private readonly Dictionary<GameEntityId, Entry> _map = new Dictionary<GameEntityId, Entry>();

        public void Set(GameEntityId id, ActionState action, int durationTicks)
        {
            if (durationTicks < 0) durationTicks = 0;
            _map[id] = new Entry { Action = action, DurationTicks = durationTicks };
        }

        public bool TryConsume(GameEntityId id, out ActionState action, out int durationTicks)
        {
            if (_map.TryGetValue(id, out var e))
            {
                _map.Remove(id);
                action = e.Action;
                durationTicks = e.DurationTicks;
                return true;
            }

            action = ActionState.None;
            durationTicks = 0;
            return false;
        }

        public void Remove(GameEntityId id) => _map.Remove(id);
        public void Clear() => _map.Clear();
    }
}