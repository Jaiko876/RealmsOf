namespace Riftborne.Core.Physics.Abstractions
{

    /// <summary>
    /// Physics backend contract. Implementations live outside Core (e.g. Unity2D).
    /// </summary>
    public interface IPhysicsWorld
    {
        void Step(float dt, int substeps);
    }
}
