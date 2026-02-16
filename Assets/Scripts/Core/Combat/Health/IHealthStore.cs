using Game.Core.Model;

namespace Game.Core.Combat.Health
{
    public interface IHealthStore
    {
        bool Has(GameEntityId entityId);

        HealthState GetOrCreate(GameEntityId entityId, float initialHp);

        bool TryGet(GameEntityId entityId, out HealthState state);

        void Set(GameEntityId entityId, HealthState state);

        void Remove(GameEntityId entityId);
    }
}
