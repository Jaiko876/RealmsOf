using Game.Core.Model;

namespace Game.Core.Physics.Abstractions
{

    /// <summary>
    /// Grounded query for platformer motor (kept as an abstraction so Core stays Unity-free).
    /// </summary>
    public interface IGroundSensor
    {
        bool IsGrounded(PlayerId playerId);
    }
}
