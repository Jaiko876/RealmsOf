using Riftborne.Core.Model;
using Riftborne.Core.Model.Animation;

namespace Riftborne.App.Animation.Provider
{
    public interface IAnimationModifiersProvider
    {
        AnimationModifiers Get(GameEntityId entityId);
    }
}