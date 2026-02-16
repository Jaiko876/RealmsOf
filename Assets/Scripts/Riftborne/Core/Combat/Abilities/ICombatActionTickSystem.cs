using Riftborne.Core.Combat.Resolution;

namespace Riftborne.Core.Combat.Abilities
{
    public interface ICombatActionTickSystem
    {
        void Tick(int tick);
    }

    public sealed class CombatActionTickSystem : ICombatActionTickSystem
    {
        private readonly ICombatActionStore _store;
        private readonly ICombatResolutionSystem _resolutionSystem;


        public CombatActionTickSystem(
            ICombatActionStore store,
            ICombatResolutionSystem resolutionSystem)
        {
            _store = store;
            _resolutionSystem = resolutionSystem;
        }

        public void Tick(int tick)
        {
            var actions = _store.All;

            for (int i = actions.Count - 1; i >= 0; i--)
            {
                var action = actions[i];
                action.Tick(tick);

                if (action is AttackAction attack && attack.IsActive)
                {
                    _resolutionSystem.ResolveAttack(attack, tick);
                }

                if (action.IsFinished(tick))
                    _store.Remove(action);
            }
        }
    }
}
