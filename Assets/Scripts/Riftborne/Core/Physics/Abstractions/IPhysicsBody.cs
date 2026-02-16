namespace Riftborne.Core.Physics.Abstractions
{

    /// <summary>
    /// Minimal mutable body state used by gameplay systems.
    /// Use plain floats to keep Core independent from Unity types.
    /// </summary>
    public interface IPhysicsBody
    {
        float X { get; set; }
        float Y { get; set; }

        float Vx { get; set; }
        float Vy { get; set; }

        void AddImpulse(float ix, float iy);
    }
}
