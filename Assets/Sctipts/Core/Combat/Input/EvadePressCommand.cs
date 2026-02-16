using Game.Core.Abstractions;
using Game.Core.Model;

namespace Game.Core.Combat.Input
{
    public readonly struct EvadePressCommand : ICommand
    {
        public int Tick { get; }
        public GameEntityId EntityId { get; }
        public float DirX { get; } // -1..1 (stick or dpad)

        public EvadePressCommand(int tick, GameEntityId entityId, float dirX)
        {
            Tick = tick;
            EntityId = entityId;
            DirX = dirX;
        }
    }
}
