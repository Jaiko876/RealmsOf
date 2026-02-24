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

            // 1) Находим аватар игрока (entityId)
            if (!_hasEntity)
            {
                if (!_state.PlayerAvatars.TryGet(_playerId, out _entityId))
                    return;

                _hasEntity = true;
            }

            // 2) Читаем статы и обновляем бары
            if (!_stats.TryGet(_entityId, out var s) || !s.IsInitialized)
                return;

            if (hpBar != null)
                hpBar.Set(s.HpCur, s.HpMax);

            if (staminaBar != null)
                staminaBar.Set(s.StaminaCur, s.StaminaMax);
        }

        private static float SafeDiv01(int cur, int max)
        {
            if (max <= 0) return 0f;
            float v = cur / (float)max;
            if (v < 0f) return 0f;
            if (v > 1f) return 1f;
            return v;
        }
    }
}