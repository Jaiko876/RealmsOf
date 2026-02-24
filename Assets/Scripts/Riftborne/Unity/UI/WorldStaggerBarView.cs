using Riftborne.Core.Model;
using Riftborne.Core.Stores.Abstractions;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Riftborne.Unity.UI
{
    public sealed class WorldStaggerBarView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image fillImage;          // Filled / Horizontal / Left
        [SerializeField] private GameObject graphicsRoot;  // ВЫКЛ/ВКЛ только графику, не этот GO

        [Header("Behavior")]
        [SerializeField] private bool hideWhenZero = true;
        [SerializeField] private float smoothFillPerSecond = 8f;

        private IStatsStore _stats;
        private GameEntityId _id;
        private bool _hasId;

        private float _shownFill01;
        private bool _visible;

        [Inject]
        public void Construct(IStatsStore stats) => _stats = stats;

        public void Bind(GameEntityId id)
        {
            _id = id;
            _hasId = true;
        }

        private void Awake()
        {
            if (graphicsRoot == null)
                graphicsRoot = gameObject; // fallback, но лучше назначить в инспекторе

            _shownFill01 = 0f;
            if (fillImage != null) fillImage.fillAmount = 0f;

            // форс выключение на старте
            _visible = true;
            SetVisible(false);
        }

        private void LateUpdate()
        {
            if (!_hasId || _stats == null || fillImage == null)
                return;

            if (!_stats.TryGet(_id, out var s) || !s.IsInitialized)
            {
                SetVisible(false);
                return;
            }

            int cur = s.StaggerCur;
            int max = s.StaggerMax;
            if (max <= 0) max = 1;

            if (hideWhenZero && cur <= 0)
            {
                SetVisible(false);
                _shownFill01 = 0f;
                fillImage.fillAmount = 0f;
                return;
            }

            SetVisible(true);

            float target01 = Mathf.Clamp01(cur / (float)max);
            float step = smoothFillPerSecond <= 0f ? 1f : smoothFillPerSecond * Time.deltaTime;

            _shownFill01 = Mathf.MoveTowards(_shownFill01, target01, step);
            fillImage.fillAmount = _shownFill01;
        }

        private void SetVisible(bool v)
        {
            if (_visible == v) return;
            _visible = v;

            if (graphicsRoot != null)
                graphicsRoot.SetActive(v);
        }
    }
}