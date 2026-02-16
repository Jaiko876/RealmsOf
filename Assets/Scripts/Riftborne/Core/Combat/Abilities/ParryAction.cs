using Riftborne.Core.Model;

namespace Riftborne.Core.Combat.Abilities
{
    public sealed class ParryAction : CombatAction
    {
        private readonly int _activeUntilTick;

        public ParryAction(GameEntityId owner, int startTick, int activeTicks)
            : base(owner, startTick)
        {
            _activeUntilTick = startTick + activeTicks;
            EndTick = _activeUntilTick;
        }

        public override void Tick(int currentTick)
        {
        }

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
