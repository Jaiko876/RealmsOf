namespace Riftborne.Core.Combat.Config
{
    public sealed class HitQueryTuning
    {
        public float HitWidth { get; }
        public float HitHeight { get; }
        public float ForwardOffset { get; }
        public int HitMask { get; }

        public HitQueryTuning(float hitWidth, float hitHeight, float forwardOffset, int hitMask)
        {
            HitWidth = hitWidth;
            HitHeight = hitHeight;
            ForwardOffset = forwardOffset;
            HitMask = hitMask;
        }
    }
}
