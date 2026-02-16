using System.Collections.Generic;
using Riftborne.Core.Model;

namespace Riftborne.Core.Combat.Resources
{
    public sealed class InMemoryCombatResourceStore : ICombatResourceStore
    {
        private readonly Dictionary<GameEntityId, StaminaState> _stamina
            = new Dictionary<GameEntityId, StaminaState>(256);

        private readonly Dictionary<GameEntityId, StaggerState> _stagger
            = new Dictionary<GameEntityId, StaggerState>(256);

        public bool Has(GameEntityId entityId)
        {
            return _stamina.ContainsKey(entityId) || _stagger.ContainsKey(entityId);
        }

        public bool TryGetStamina(GameEntityId entityId, out StaminaState stamina)
        {
            return _stamina.TryGetValue(entityId, out stamina);
        }

        public void SetStamina(GameEntityId entityId, StaminaState stamina)
        {
            _stamina[entityId] = stamina;
        }

        public bool TryGetStagger(GameEntityId entityId, out StaggerState stagger)
        {
            return _stagger.TryGetValue(entityId, out stagger);
        }

        public void SetStagger(GameEntityId entityId, StaggerState stagger)
        {
            _stagger[entityId] = stagger;
        }

        public void Remove(GameEntityId entityId)
        {
            _stamina.Remove(entityId);
            _stagger.Remove(entityId);
        }
    }
}
