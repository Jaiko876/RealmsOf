using Riftborne.Core.Model;

namespace Riftborne.Core.Combat.Abilities
{
    /// <summary>
    /// Простая v1: блок активен фиксированное число тиков.
    /// Позже сделаем "hold-to-sustain" через отдельную систему/инпут стейт.
    /// </summary>
    public sealed class BlockAction : CombatAction
    {
        private readonly int _activeUntilTick;

        public BlockAction(GameEntityId owner, int startTick, int activeTicks)
            : base(owner, startTick)
        {
            _activeUntilTick = startTick + activeTicks;
            EndTick = _activeUntilTick;
        }

        public override void Tick(int currentTick) { }

        public override bool IsFinished(int currentTick)
        {
            return currentTick >= EndTick;
        }

        public bool IsActive(int currentTick)
        {
            return currentTick < _activeUntilTick;
        }
    }
}
