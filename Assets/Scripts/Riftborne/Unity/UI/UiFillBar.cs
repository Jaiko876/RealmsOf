using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Riftborne.Unity.UI
{
    public sealed class UiFillBar : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private TMP_Text valueText;

        private int _lastCur = -1;
        private int _lastMax = -1;

        public void Set(int cur, int max)
        {
            if (max <= 0) max = 1;

            float value01 = cur / (float)max;
            if (value01 < 0f) value01 = 0f;
            if (value01 > 1f) value01 = 1f;

            if (fillImage != null)
                fillImage.fillAmount = value01;

            // Чтобы не форматировать строку каждый кадр
            if (valueText != null && (cur != _lastCur || max != _lastMax))
            {
                valueText.text = $"{cur}/{max}";
                _lastCur = cur;
                _lastMax = max;
            }
        }
    }
}