using Riftborne.Core.Model;

namespace Riftborne.Core.Combat.Resources
{
    public interface ICombatResourceStore
    {
        bool Has(GameEntityId entityId);

        bool TryGetStamina(GameEntityId entityId, out StaminaState stamina);
        void SetStamina(GameEntityId entityId, StaminaState stamina);

        bool TryGetStagger(GameEntityId entityId, out StaggerState stagger);
        void SetStagger(GameEntityId entityId, StaggerState stagger);

        void Remove(GameEntityId entityId);
    }
}
