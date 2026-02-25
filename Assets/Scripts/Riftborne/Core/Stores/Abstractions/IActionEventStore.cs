using Riftborne.Core.Model;
using Riftborne.Core.Model.Animation;

namespace Riftborne.Core.Stores.Abstractions
{
    public interface IActionEventStore
    {
        /// <summary>
        /// Publishes an action intent for this entity. If timing was already set for the same action,
        /// it will be merged and preserved. If a different action was pending, it will be replaced.
        /// </summary>
        void SetIntent(GameEntityId id, ActionState action, int tick);

        /// <summary>
        /// Publishes timing for a specific action. If intent was already set for the same action,
        /// it will be merged. If intent is not set yet, timing will be kept until intent arrives.
        /// If a different action was pending, it will be replaced.
        /// </summary>
        void SetTiming(GameEntityId id, ActionState action, int durationTicks, int tick);

        /// <summary>
        /// Consumes the pending action event if intent exists.
        /// Timing is optional and returned as DurationTicks (0 if not provided).
        /// </summary>
        bool TryConsume(GameEntityId id, int tick, out ActionEvent e);

        void Remove(GameEntityId id);
        void Clear();
    }
}