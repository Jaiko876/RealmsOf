namespace Riftborne.Core.Gameplay.Locomotion.Model
{
    public readonly struct LocomotionConstraints
    {
        public readonly bool HasFacingLock;
        public readonly sbyte FacingLock; // -1/+1

        // NEW: when facing is locked, optionally forbid moving against it (prevents moonwalk)
        public readonly bool ForbidMoveAgainstFacing;

        // Multipliers (1 = no change). Applied on top of Stats->Physics.
        public readonly float MoveSpeedMul;
        public readonly float AccelMul;
        public readonly float DecelMul;

        public LocomotionConstraints(
            bool hasFacingLock,
            sbyte facingLock,
            bool forbidMoveAgainstFacing,
            float moveSpeedMul,
            float accelMul,
            float decelMul)
        {
            HasFacingLock = hasFacingLock;
            FacingLock = (sbyte)(facingLock < 0 ? -1 : 1);
            ForbidMoveAgainstFacing = forbidMoveAgainstFacing;

            MoveSpeedMul = moveSpeedMul <= 0f ? 1f : moveSpeedMul;
            AccelMul = accelMul <= 0f ? 1f : accelMul;
            DecelMul = decelMul <= 0f ? 1f : decelMul;
        }

        // Backward-compatible ctor (old call sites)
        public LocomotionConstraints(
            bool hasFacingLock,
            sbyte facingLock,
            float moveSpeedMul,
            float accelMul,
            float decelMul)
            : this(hasFacingLock, facingLock, false, moveSpeedMul, accelMul, decelMul)
        {
        }

        public static LocomotionConstraints None =>
            new LocomotionConstraints(false, 1, false, 1f, 1f, 1f);
    }
}