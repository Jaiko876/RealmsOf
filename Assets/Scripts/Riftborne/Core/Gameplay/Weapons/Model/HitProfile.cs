namespace Riftborne.Core.Gameplay.Weapons.Model
{
    /// <summary>
    /// Deterministic hit shape in entity-local space (no Unity Transform).
    /// Offsets are relative to entity pivot (EntityState.X/Y).
    /// Facing is applied by mirroring X.
    /// </summary>
    public readonly struct HitProfile
    {
        public readonly float OffsetX;
        public readonly float OffsetY;

        public readonly float Width;
        public readonly float Height;

        public HitProfile(float offsetX, float offsetY, float width, float height)
        {
            OffsetX = offsetX;
            OffsetY = offsetY;
            Width = width;
            Height = height;
        }
    }
}