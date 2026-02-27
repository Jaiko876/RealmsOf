using System;
using Riftborne.Configs;
using Riftborne.Core.Model;
using Riftborne.Unity.VFX;
using Riftborne.Unity.View.Presenters.Abstractions;
using UnityEngine;
using VContainer;

namespace Riftborne.Unity.View
{
    public sealed class EntityView : MonoBehaviour
    {
        [Header("Entity Binding")] [SerializeField]
        private int entityId = 0;

        [Header("View Roots")] [SerializeField]
        private Transform visualRoot;

        [SerializeField] private Transform flipRoot;

        [Header("Animation")] [SerializeField] private Animator animator;
        [SerializeField] private ChargeFullFlashView flash;

        [Header("Config")] [SerializeField] private AttackAnimationConfigAsset attackAnimationConfig;

        private GameState _gameState;
        private GameEntityId _entityId;

        private IEntityTransformPresenter _transformPresenter;
        private IEntityAnimatorPresenter _animatorPresenter;

        [Inject]
        public void Construct(
            GameState gameState,
            AttackAnimationConfigAsset attackAnimConfig,
            IEntityTransformPresenter transformPresenter,
            IEntityAnimatorPresenter animatorPresenter)
        {
            _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
            attackAnimationConfig = attackAnimConfig ?? throw new ArgumentNullException(nameof(attackAnimConfig));
            _transformPresenter = transformPresenter ?? throw new ArgumentNullException(nameof(transformPresenter));
            _animatorPresenter = animatorPresenter ?? throw new ArgumentNullException(nameof(animatorPresenter));
        }

        private void Start()
        {
            _entityId = new GameEntityId(entityId);

            if (visualRoot == null)
                visualRoot = transform;

            if (flipRoot == null)
            {
                if (animator != null) flipRoot = animator.transform;
                else flipRoot = visualRoot;
            }

            if (attackAnimationConfig == null)
                throw new InvalidOperationException(
                    $"{nameof(EntityView)} requires {nameof(attackAnimationConfig)} on '{name}'.");

            _animatorPresenter.Initialize(animator, flash, attackAnimationConfig);

            // Ensure entity exists early (binding sanity).
            if (_gameState != null)
                _gameState.GetOrCreateEntity(_entityId);
        }

        private void LateUpdate()
        {
            if (_gameState == null)
                return;

            // In идеале тут был бы TryGetEntity, чтобы View не создавал сущности.
            // Пока используем существующий контракт.
            var e = _gameState.GetOrCreateEntity(_entityId);

            float alpha = ComputeAlpha01(Time.time, Time.fixedTime, Time.fixedDeltaTime);

            _transformPresenter.Present(e, alpha, visualRoot, flipRoot);
            _animatorPresenter.Present(e.AnimationState, e.Facing, Time.deltaTime, Time.fixedDeltaTime);
        }

        private static float ComputeAlpha01(float time, float fixedTime, float fixedDeltaTime)
        {
            if (fixedDeltaTime <= 0f) return 1f;

            var alpha = (time - fixedTime) / fixedDeltaTime;
            return Mathf.Clamp01(alpha);
        }
    }
}