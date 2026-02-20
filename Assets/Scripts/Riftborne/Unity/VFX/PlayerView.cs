using UnityEngine;

namespace Riftborne.Unity.VFX
{
    public sealed class ChargeFullFlashView : MonoBehaviour
    {
        [Header("Refs")] [SerializeField] private SpriteRenderer _flashRenderer;

        [Header("Timing")] [SerializeField] private float _durationSeconds = 0.11f;

        [Header("Look")] [SerializeField] private Color _flashColor = new Color(1f, 0.15f, 0.15f, 1f);
        [SerializeField] private float _scalePeak = 1.22f; // максимальный раздув
        [SerializeField] private float _peakAt = 0.18f; // где пик (0..1) по времени
        [SerializeField] private float _forwardOffset = 0.12f; // смещение вперёд (в локальных единицах)

        private float _t;
        private bool _playing;

        private Vector3 _baseLocalPos;
        private Vector3 _baseScale;

        // направление "вперёд" задаёшь снаружи: +1 = вправо, -1 = влево
        private int _facing = 1;

        private void Awake()
        {
            if (_flashRenderer == null)
                _flashRenderer = GetComponentInChildren<SpriteRenderer>(true);

            _baseLocalPos = _flashRenderer.transform.localPosition;
            _baseScale = _flashRenderer.transform.localScale;

            _flashRenderer.enabled = false;
        }

        public void SetFacing(int facing)
        {
            _facing = (facing >= 0) ? 1 : -1;
        }

        public void PlayOnce()
        {
            _t = 0f;
            _playing = true;

            _flashRenderer.enabled = true;
            _flashRenderer.color = _flashColor;

            // стартовая позиция — чуть впереди
            var p = _baseLocalPos;
            p.x += _forwardOffset * _facing;
            _flashRenderer.transform.localPosition = p;

            _flashRenderer.transform.localScale = _baseScale;
        }

        private void Update()
        {
            if (!_playing) return;

            _t += Time.deltaTime;
            float dur = (_durationSeconds <= 0f) ? 0.0001f : _durationSeconds;
            float u = Mathf.Clamp01(_t / dur); // 0..1

            // --- Alpha: резкий старт -> быстрое затухание (ease-out)
            // kAlpha = (1 - u)^2, но первые кадры максимально ярко
            float kAlpha = 1f - u;
            kAlpha *= kAlpha;

            // --- Scale: быстрый рост до пика, потом спад
            float s = ComputePunchScale(u, _peakAt, _scalePeak);

            // Применяем
            var c = _flashColor;
            c.a = kAlpha;
            _flashRenderer.color = c;

            _flashRenderer.transform.localScale = new Vector3(_baseScale.x * s, _baseScale.y * s, _baseScale.z);

            if (_t >= dur)
            {
                StopInternal();
            }
        }

        private void StopInternal()
        {
            _playing = false;
            _flashRenderer.enabled = false;

            _flashRenderer.transform.localPosition = _baseLocalPos;
            _flashRenderer.transform.localScale = _baseScale;
        }

        // "панч"-кривая:
        // 0..peakAt: быстро растём (ease-out), peakAt..1: быстро падаем (ease-in)
        private static float ComputePunchScale(float u01, float peakAt01, float peakScale)
        {
            if (peakAt01 <= 0f) peakAt01 = 0.0001f;
            if (peakAt01 >= 1f) peakAt01 = 0.9999f;
            if (peakScale < 1f) peakScale = 1f;

            if (u01 <= peakAt01)
            {
                float t = u01 / peakAt01; // 0..1
                float easeOut = 1f - (1f - t) * (1f - t); // quadratic ease-out
                return Mathf.Lerp(1f, peakScale, easeOut);
            }
            else
            {
                float t = (u01 - peakAt01) / (1f - peakAt01); // 0..1
                float easeIn = t * t; // quadratic ease-in
                return Mathf.Lerp(peakScale, 1f, easeIn);
            }
        }
    }
}