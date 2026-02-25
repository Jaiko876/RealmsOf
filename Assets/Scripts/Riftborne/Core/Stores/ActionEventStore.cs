using System.Collections.Generic;
using Riftborne.Core.Model;
using Riftborne.Core.Model.Animation;
using Riftborne.Core.Stores.Abstractions;

namespace Riftborne.Core.Stores
{
    public sealed class ActionEventStore : IActionEventStore
    {
        // Сколько тиков держим "timing без intent" (чтобы не висел вечным мусором).
        // Обычно достаточно 1-2, но ставлю 2 на случай переупорядочивания систем в пайплайне.
        private const int TimingTtlTicks = 2;

        private readonly Dictionary<GameEntityId, Entry> _map = new Dictionary<GameEntityId, Entry>(128);

        public void SetIntent(GameEntityId id, ActionState action, int tick)
        {
            if (action == ActionState.None) return;

            if (_map.TryGetValue(id, out var e))
            {
                // Drop stale entries
                if (IsStale(tick, e.LastUpdatedTick))
                    e = Entry.Empty;

                if (e.IsEmpty)
                {
                    _map[id] = Entry.ForIntent(action, tick);
                    return;
                }

                // Same action => keep timing if it is recent enough.
                if (e.Action == action)
                {
                    e.Action = action;
                    e.HasIntent = true;
                    e.LastUpdatedTick = tick;
                    _map[id] = e;
                    return;
                }

                // Different action => replace (policy: single action per entity per tick-window).
                _map[id] = Entry.ForIntent(action, tick);
                return;
            }

            _map[id] = Entry.ForIntent(action, tick);
        }

        public void SetTiming(GameEntityId id, ActionState action, int durationTicks, int tick)
        {
            if (action == ActionState.None) return;
            if (durationTicks < 0) durationTicks = 0;

            if (_map.TryGetValue(id, out var e))
            {
                // Drop stale entries
                if (IsStale(tick, e.LastUpdatedTick))
                    e = Entry.Empty;

                if (e.IsEmpty)
                {
                    _map[id] = Entry.ForTiming(action, durationTicks, tick);
                    return;
                }

                // Same action => attach/overwrite timing, keep intent flag.
                if (e.Action == action)
                {
                    e.Action = action;
                    e.HasTiming = true;
                    e.DurationTicks = durationTicks;
                    e.LastUpdatedTick = tick;
                    _map[id] = e;
                    return;
                }

                // Different action => replace
                _map[id] = Entry.ForTiming(action, durationTicks, tick);
                return;
            }

            _map[id] = Entry.ForTiming(action, durationTicks, tick);
        }

        public bool TryConsume(GameEntityId id, int tick, out ActionEvent e)
        {
            if (_map.TryGetValue(id, out var entry))
            {
                // Если запись устарела — выкидываем и не отдаём событие.
                if (IsStale(tick, entry.LastUpdatedTick))
                {
                    _map.Remove(id);
                    e = ActionEvent.None;
                    return false;
                }

                if (entry.HasIntent)
                {
                    _map.Remove(id);

                    var duration = entry.HasTiming ? entry.DurationTicks : 0;
                    e = new ActionEvent(entry.Action, duration);
                    return true;
                }

                // Timing без intent держим ограниченно.
                if (tick - entry.LastUpdatedTick >= TimingTtlTicks)
                    _map.Remove(id);
            }

            e = ActionEvent.None;
            return false;
        }

        public void Remove(GameEntityId id) => _map.Remove(id);
        public void Clear() => _map.Clear();

        private static bool IsStale(int nowTick, int lastTick)
        {
            // lastTick == 0 when empty; allow 0 as empty marker
            if (lastTick == 0) return false;
            return (nowTick - lastTick) > TimingTtlTicks;
        }

        private struct Entry
        {
            public ActionState Action;
            public bool HasIntent;
            public bool HasTiming;
            public int DurationTicks;
            public int LastUpdatedTick;

            public bool IsEmpty => Action == ActionState.None && !HasIntent && !HasTiming && DurationTicks == 0 && LastUpdatedTick == 0;

            public static Entry Empty => new Entry
            {
                Action = ActionState.None,
                HasIntent = false,
                HasTiming = false,
                DurationTicks = 0,
                LastUpdatedTick = 0
            };

            public static Entry ForIntent(ActionState action, int tick)
            {
                return new Entry
                {
                    Action = action,
                    HasIntent = true,
                    HasTiming = false,
                    DurationTicks = 0,
                    LastUpdatedTick = tick
                };
            }

            public static Entry ForTiming(ActionState action, int durationTicks, int tick)
            {
                return new Entry
                {
                    Action = action,
                    HasIntent = false,
                    HasTiming = true,
                    DurationTicks = durationTicks,
                    LastUpdatedTick = tick
                };
            }
        }
    }
}