using Riftborne.Core.Model;
using Riftborne.Core.Model.Animation;

namespace Riftborne.Core.Animation.Abstractions
{
    public interface IAnimationModifiersProvider
    {
        AnimationModifiers Get(GameEntityId entityId);
    }
}