using Riftborne.Core.Model;
using Riftborne.Core.Stats;

namespace Riftborne.Core.Stores.Abstractions
{
    public interface IStatsStore
    {
        StatsState GetOrCreate(GameEntityId id);
        bool TryGet(GameEntityId id, out StatsState state);
        void Remove(GameEntityId id);
        void Clear();
    }
}