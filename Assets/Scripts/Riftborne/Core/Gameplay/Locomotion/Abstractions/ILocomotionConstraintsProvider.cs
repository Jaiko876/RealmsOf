using Riftborne.Core.Gameplay.Locomotion.Model;
using Riftborne.Core.Model;

namespace Riftborne.Core.Gameplay.Locomotion.Abstractions
{
    public interface ILocomotionConstraintsProvider
    {
        LocomotionConstraints Get(GameEntityId id, int tick);
    }
}