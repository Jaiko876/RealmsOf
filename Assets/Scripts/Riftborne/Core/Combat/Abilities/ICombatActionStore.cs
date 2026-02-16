using System.Collections.Generic;

namespace Riftborne.Core.Combat.Abilities
{
    public interface ICombatActionStore
    {
        void Add(CombatAction action);
        IReadOnlyList<CombatAction> All { get; }
        void Remove(CombatAction action);
    }

    public sealed class InMemoryCombatActionStore : ICombatActionStore
    {
        private readonly List<CombatAction> _actions = new List<CombatAction>(64);

        public void Add(CombatAction action)
        {
            _actions.Add(action);
        }

        public IReadOnlyList<CombatAction> All => _actions;

        public void Remove(CombatAction action)
        {
            _actions.Remove(action);
        }
    }
}
