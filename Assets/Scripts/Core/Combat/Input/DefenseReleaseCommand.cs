using Game.Core.Abstractions;
using Game.Core.Model;

namespace Game.Core.Combat.Input
{
    public readonly struct DefenseReleaseCommand : ICommand
    {
        public int Tick { get; }
        public GameEntityId EntityId { get; }

        public DefenseReleaseCommand(int tick, GameEntityId entityId)
        {
            Tick = tick;
            EntityId = entityId;
        }
    }
}
