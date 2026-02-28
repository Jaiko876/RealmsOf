using System;
using Riftborne.Core.Model;
using Riftborne.Unity.View.Registry;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

namespace Riftborne.Unity.Bootstrap
{
    public sealed class LocalPlayerCameraBinder : MonoBehaviour
    {
        [SerializeField] private int playerId = 0;
        [SerializeField] private CinemachineCamera cameraToBind;

        private GameState _state;
        private IEntityViewRegistry _views;

        private PlayerId _playerId;
        private GameEntityId _boundEntity;
        private bool _hasBound;

        [Inject]
        public void Construct(GameState state, IEntityViewRegistry views)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _views = views ?? throw new ArgumentNullException(nameof(views));
        }

        private void Awake()
        {
            _playerId = new PlayerId(playerId);
        }

        private void LateUpdate()
        {
            if (cameraToBind == null)
                return;

            if (!_state.PlayerAvatars.TryGet(_playerId, out var entityId))
            {
                ClearCameraTargets();
                return;
            }

            // already bound to same entity and follow is still set
            if (_hasBound && _boundEntity.Equals(entityId) && cameraToBind.Follow != null)
                return;

            if (_views.TryGetFollowTarget(entityId, out var target) && target != null)
            {
                SetCameraTargets(target);
                _boundEntity = entityId;
                _hasBound = true;
            }
        }

        private void SetCameraTargets(Transform target)
        {
            // Cinemachine 3: Tracking Target maps to Follow (and optionally LookAt)
            cameraToBind.Follow = target;
            cameraToBind.LookAt = target;
        }

        private void ClearCameraTargets()
        {
            _hasBound = false;
            cameraToBind.Follow = null;
            cameraToBind.LookAt = null;
        }
    }
}