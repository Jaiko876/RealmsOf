using Riftborne.Core.Model;

namespace Riftborne.Core.Stores.Abstractions
{
    public interface IDefenseHoldStore
    {
        bool TryGet(GameEntityId id, out bool prevHeld, out int heldTicks);
        void Set(GameEntityId id, bool prevHeld, int heldTicks);

        void Remove(GameEntityId id);
        void Clear();
    }
}