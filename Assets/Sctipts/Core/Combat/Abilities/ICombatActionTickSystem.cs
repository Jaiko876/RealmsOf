using Game.Core.Combat.Damage;
using Game.Core.Model;

namespace Game.Core.Combat.Abilities
{
    public interface ICombatActionTickSystem
    {
        void Tick(int tick);
    }

    public sealed class CombatActionTickSystem : ICombatActionTickSystem
    {
        private readonly ICombatActionStore _store;
        private readonly IHealthDamageService _damageService;

        public CombatActionTickSystem(
            ICombatActionStore store,
            IHealthDamageService damageService)
        {
            _store = store;
            _damageService = damageService;
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
                    // Пока без hit detection — self-target для теста
                    var request = new DamageRequest(
                        attacker: attack.Owner,
                        target: attack.Owner,
                        baseHpDamage: 1f,
                        baseStaminaDamage: 0f,
                        baseStaggerBuild: 0f);

                    _damageService.Apply(request, tick);
                }

                if (action.IsFinished(tick))
                    _store.Remove(action);
            }
        }
    }
}
