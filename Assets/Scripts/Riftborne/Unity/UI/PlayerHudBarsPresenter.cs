using Riftborne.Core.Model;
using Riftborne.Core.Stores.Abstractions;
using UnityEngine;
using VContainer;

namespace Riftborne.Unity.UI
{
    public sealed class PlayerHudBarsPresenter : MonoBehaviour
    {
        [Header("Bind")]
        [SerializeField] private int playerId = 0;

        [Header("HUD Bars")]
        [SerializeField] private UiFillBar hpBar;
        [SerializeField] private UiFillBar staminaBar;

        private GameState _state;
        private IStatsStore _stats;

        private PlayerId _playerId;
        private GameEntityId _entityId;
        private bool _hasEntity;

        [Inject]
        public void Construct(GameState state, IStatsStore stats)
        {
            _state = state;
            _stats = stats;
        }

        private void Awake()
        {
            _playerId = new PlayerId(playerId);
        }

        private void LateUpdate()
        {
            if (_state == null || _stats == null)
                return;

            if (!_state.PlayerAvatars.TryGet(_playerId, out var current))
            {
                _hasEntity = false;
                return;
            }

            if (!_hasEntity || !_entityId.Equals(current))
            {
                _entityId = current;
                _hasEntity = true;
            }

            if (!_stats.TryGet(_entityId, out var s) || !s.IsInitialized)
                return;

            if (hpBar != null) hpBar.Set(s.HpCur, s.HpMax);
            if (staminaBar != null) staminaBar.Set(s.StaminaCur, s.StaminaMax);
        }
    }
}