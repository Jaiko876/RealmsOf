using Riftborne.Core.Model.Animation;

namespace Riftborne.App.Animation.Composition.Abstractions
{
    public interface IAnimationStateComposer
    {
        AnimationState Compose(in AnimationStateComposeContext ctx);
    }
}