using Game.Core.Model;

namespace Game.Core.Combat.Input
{
    public readonly struct BufferedIntent
    {
        public GameEntityId EntityId { get; }
        public CombatIntent Intent { get; }
        public int ExpiresAtTick { get; }
        public float DirX { get; } // for dash; 0 for others

        public BufferedIntent(GameEntityId entityId, CombatIntent intent, int expiresAtTick, float dirX)
        {
            EntityId = entityId;
            Intent = intent;
            ExpiresAtTick = expiresAtTick;
            DirX = dirX;
        }
    }
}
