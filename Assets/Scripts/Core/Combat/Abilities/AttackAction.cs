using Game.Core.Model;

namespace Game.Core.Combat.Abilities
{
    public sealed class AttackAction : CombatAction
    {
        private readonly AbilityDefinition _definition;

        public bool IsActive { get; private set; }

        public AttackAction(GameEntityId owner, int startTick, AbilityDefinition definition)
            : base(owner, startTick)
        {
            _definition = definition;
            EndTick = startTick + definition.WindupTicks
                                    + definition.ActiveTicks
                                    + definition.RecoveryTicks;
        }

        public override void Tick(int currentTick)
        {
            int activeStart = StartTick + _definition.WindupTicks;
            int activeEnd = activeStart + _definition.ActiveTicks;

            IsActive = currentTick >= activeStart && currentTick < activeEnd;
        }

        public override bool IsFinished(int currentTick)
        {
            return currentTick >= EndTick;
        }

        public AbilityDefinition Definition => _definition;
    }
}
