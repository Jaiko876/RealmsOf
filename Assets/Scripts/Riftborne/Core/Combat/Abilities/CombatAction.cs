using Riftborne.Core.Model;

namespace Riftborne.Core.Combat.Abilities
{
    public abstract class CombatAction
    {
        public GameEntityId Owner { get; }

        public int StartTick { get; }
        public int EndTick { get; protected set; }

        protected CombatAction(GameEntityId owner, int startTick)
        {
            Owner = owner;
            StartTick = startTick;
        }

        public abstract void Tick(int currentTick);
        public abstract bool IsFinished(int currentTick);
    }
}
