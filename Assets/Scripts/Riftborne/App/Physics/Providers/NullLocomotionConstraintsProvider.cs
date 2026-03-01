using Riftborne.Core.Gameplay.Locomotion.Abstractions;
using Riftborne.Core.Gameplay.Locomotion.Model;
using Riftborne.Core.Model;

namespace Riftborne.App.Physics.Providers
{
    public sealed class NullLocomotionConstraintsProvider : ILocomotionConstraintsProvider
    {
        public LocomotionConstraints Get(GameEntityId id, int tick) => LocomotionConstraints.None;
    }
}