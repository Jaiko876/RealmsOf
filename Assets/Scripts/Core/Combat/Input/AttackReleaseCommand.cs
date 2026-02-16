using Game.Core.Abstractions;
using Game.Core.Model;

namespace Game.Core.Combat.Input
{
    public readonly struct AttackReleaseCommand : ICommand
    {
        public int Tick { get; }
        public GameEntityId EntityId { get; }

        public AttackReleaseCommand(int tick, GameEntityId entityId)
        {
            Tick = tick;
            EntityId = entityId;
        }
    }
}
