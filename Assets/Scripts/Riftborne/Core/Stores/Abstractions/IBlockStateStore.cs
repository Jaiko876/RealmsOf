using Riftborne.Core.Model;

namespace Riftborne.Core.Stores.Abstractions
{
    public interface IBlockStateStore
    {
        bool IsBlocking(GameEntityId id);
        void SetBlocking(GameEntityId id, bool blocking);

        void Remove(GameEntityId id);
        void Clear();
    }
}