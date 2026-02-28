using Riftborne.Core.Model;
using Riftborne.Core.Stores.Abstractions;
using UnityEngine;
using VContainer;

namespace Riftborne.Unity.Debugging
{
    public sealed class PlayerStatsDebugLog : MonoBehaviour
    {
        [SerializeField] private int playerId = 0;
        [SerializeField] private int logEveryFrames = 30;

        private GameState _state;
        private IStatsStore _stats;

        private PlayerId _playerId;

        private int _frame;

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

        private void Update()
        {
            _frame++;
            if (logEveryFrames <= 0) logEveryFrames = 30;
            if ((_frame % logEveryFrames) != 0)
                return;

            if (_state == null || _stats == null)
                return;

            if (!_state.PlayerAvatars.TryGet(_playerId, out var entityId))
            {
                Debug.Log($"[StatsDbg] no avatar for playerId={playerId}");
                return;
            }

            if (!_stats.TryGet(entityId, out var s))
            {
                Debug.Log($"[StatsDbg] no stats for entityId={entityId.Value}");
                return;
            }

            Debug.Log(
                $"[StatsDbg] p={playerId} e={entityId.Value} init={s.IsInitialized} " +
                $"HP {s.HpCur}/{s.HpMax} | STA {s.StaminaCur}/{s.StaminaMax} | STG {s.StaggerCur}/{s.StaggerMax}");
        }
    }
}