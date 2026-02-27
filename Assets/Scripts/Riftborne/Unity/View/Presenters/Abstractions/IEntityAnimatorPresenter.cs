using Riftborne.Configs;
using Riftborne.Unity.VFX;
using UnityEngine;
using AnimationState = Riftborne.Core.Model.Animation.AnimationState;

namespace Riftborne.Unity.View.Presenters.Abstractions
{
    public interface IEntityAnimatorPresenter
    {
        void Initialize(Animator animator, ChargeFullFlashView flash, AttackAnimationConfigAsset config);
        void Present(AnimationState a, int facing, float dt, float fixedDt);
    }
}