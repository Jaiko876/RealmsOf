using UnityEngine;

namespace Riftborne.Unity.Render
{
    public sealed class RenderAlphaClock : MonoBehaviour, IRenderAlphaProvider
    {
        [SerializeField] private float tickDt = 1f / 60f;

        private float _accumulator;

        public float Alpha
        {
            get
            {
                if (tickDt <= 0f) return 1f;
                var a = _accumulator / tickDt;
                if (a < 0f) return 0f;
                if (a > 1f) return 1f;
                return a;
            }
        }

        public void AddDelta(float dt)
        {
            _accumulator += dt;
        }

        public void ConsumeTick(float tickDtValue)
        {
            _accumulator -= tickDtValue;
            if (_accumulator < 0f) _accumulator = 0f;
        }
    }
}
