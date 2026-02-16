using Riftborne.Core.Combat.Abilities;

namespace Riftborne.Core.Combat.Resolution
{
    /// <summary>
    /// Отвечает за резолв атакующих действий против целей.
    /// Детерминированно по тику.
    /// </summary>
    public interface ICombatResolutionSystem
    {
        /// <summary>
        /// Вызывается в active-фазе атаки.
        /// Производит hit query и применяет правила Light/Heavy/Parry/Dodge.
        /// </summary>
        void ResolveAttack(AttackAction attack, int tick);
    }
}
