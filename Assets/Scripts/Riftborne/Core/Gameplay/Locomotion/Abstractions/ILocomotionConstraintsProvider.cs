using Riftborne.Core.Model;

namespace Riftborne.Core.Gameplay.Locomotion.Abstractions
{
    public interface ILocomotionConstraintsProvider
    {
        LocomotionConstraints Get(GameEntityId id, int tick);
    }

    public readonly struct LocomotionConstraints
    {
        public readonly bool HasFacingLock;
        public readonly sbyte FacingLock;      // -1/+1

        // Multipliers (1 = no change). Applied on top of Stats->Physics.
        public readonly float MoveSpeedMul;
        public readonly float AccelMul;
        public readonly float DecelMul;

        public LocomotionConstraints(
            bool hasFacingLock,
            sbyte facingLock,
            float moveSpeedMul,
            float accelMul,
            float decelMul)
        {
            HasFacingLock = hasFacingLock;
            FacingLock = (sbyte)(facingLock < 0 ? -1 : 1);

            MoveSpeedMul = moveSpeedMul <= 0f ? 1f : moveSpeedMul;
            AccelMul = accelMul <= 0f ? 1f : accelMul;
            DecelMul = decelMul <= 0f ? 1f : decelMul;
        }

        public static LocomotionConstraints None =>
            new LocomotionConstraints(false, 1, 1f, 1f, 1f);
    }
}