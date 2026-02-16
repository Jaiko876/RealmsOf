using Game.Core.Model;

namespace Game.Core.Combat.Abilities
{
    public sealed class DodgeAction : CombatAction
    {
        private readonly int _invulnerableUntil;

        public DodgeAction(GameEntityId owner, int startTick, int activeTicks)
            : base(owner, startTick)
        {
            _invulnerableUntil = startTick + activeTicks;
            EndTick = _invulnerableUntil;
        }

        public override void Tick(int currentTick)
        {
        }

        public override bool IsFinished(int currentTick)
        {
            return currentTick >= EndTick;
        }

        public bool IsInvulnerable(int currentTick)
        {
            return currentTick < _invulnerableUntil;
        }
    }
}
