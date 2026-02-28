using System;
using Riftborne.Configs;
using Riftborne.Core.Entities;
using Riftborne.Core.Model;
using Riftborne.Unity.VFX;
using Riftborne.Unity.View.Presenters.Abstractions;
using Riftborne.Unity.View.Registry;
using UnityEngine;
using VContainer;

namespace Riftborne.Unity.View
{
    public sealed class EntityView : MonoBehaviour, IGameEntityIdReceiver
    {
        [Header("Scene fallback (debug only)")]
        [SerializeField] private int sceneEntityId = 0;

        [Header("View Roots")]
        [SerializeField] private Transform visualRoot;
        [SerializeField] private Transform flipRoot;

        [Header("Animation")]
        [SerializeField] private Animator animator;
        [SerializeField] private ChargeFullFlashView flash;

        [Header("Config")]
        [SerializeField] private AttackAnimationConfigAsset attackAnimationConfig;

        private GameState _gameState;
        private IEntityViewRegistry _views;

        private GameEntityId _entityId;
        private bool _hasId;

        private IEntityTransformPresenter _transformPresenter;
        private IEntityAnimatorPresenter _animatorPresenter;

        private bool _registered;

        [Inject]
        public void Construct(
            GameState gameState,
            AttackAnimationConfigAsset attackAnimConfig,
            IEntityTransformPresenter transformPresenter,
            IEntityAnimatorPresenter animatorPresenter,
            IEntityViewRegistry views)
        {
            _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
            attackAnimationConfig = attackAnimConfig ?? throw new ArgumentNullException(nameof(attackAnimConfig));
            _transformPresenter = transformPresenter ?? throw new ArgumentNullException(nameof(transformPresenter));
            _animatorPresenter = animatorPresenter ?? throw new ArgumentNullException(nameof(animatorPresenter));
            _views = views ?? throw new ArgumentNullException(nameof(views));
        }

        public void SetEntityId(GameEntityId id)
        {
            _entityId = id;
            _hasId = true;
            sceneEntityId = id.Value;
        }

        private void Start()
        {
            if (!_hasId)
            {
                _entityId = new GameEntityId(sceneEntityId);
                _hasId = true;
            }

            if (visualRoot == null)
                visualRoot = transform;

            if (flipRoot == null)
                flipRoot = animator != null ? animator.transform : visualRoot;

            _animatorPresenter.Initialize(animator, flash, attackAnimationConfig);

            if (_gameState == null || !_gameState.Entities.ContainsKey(_entityId))
            {
                Debug.LogError($"EntityView '{name}': no EntityState for entityId={_entityId.Value}.", this);
                enabled = false;
                return;
            }

            // Register follow target for camera etc.
            _views.RegisterFollowTarget(_entityId, visualRoot);
            _registered = true;
        }

        private void OnDestroy()
        {
            if (_registered && _views != null)
                _views.UnregisterFollowTarget(_entityId, visualRoot);
        }

        private void LateUpdate()
        {
            if (_gameState == null)
                return;

            if (!_gameState.Entities.TryGetValue(_entityId, out var e) || e == null)
                return;

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