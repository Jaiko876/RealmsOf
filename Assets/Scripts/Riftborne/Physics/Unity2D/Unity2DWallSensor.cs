using Riftborne.Core.Model;
using Riftborne.Core.Physics.Abstractions;
using UnityEngine;

namespace Riftborne.Physics.Unity2D
{
    public sealed class Unity2DWallSensor : IWallSensor
    {
        private readonly IBodyProvider<GameEntityId> _bodies;
        private readonly ContactFilter2D _filter;
        private readonly RaycastHit2D[] _hits = new RaycastHit2D[8];

        // Чуть внутрь коллайдера, чтобы не ловить “самого себя”
        private const float Skin = 0.02f;

        // Насколько “в сторону” проверяем (обычно 0.05..0.12)
        private const float CheckDistance = 0.08f;

        // Узкая толщина “щупа” стены (по X)
        private const float ProbeThickness = 0.02f;

        // Насколько уменьшаем высоту, чтобы не цеплять пол/потолок на углах
        private const float HeightShrink = 0.12f; // доля от высоты (0..0.3)

        // Нормаль должна смотреть “достаточно” влево/вправо
        private const float MinWallNormalAbsX = 0.6f;

        public Unity2DWallSensor(IBodyProvider<GameEntityId> bodies)
        {
            _bodies = bodies;

            _filter = new ContactFilter2D();
            _filter.useLayerMask = true;
            _filter.layerMask = LayerMask.GetMask("Ground");
            _filter.useTriggers = false;
        }

        public bool IsBlockedLeft(GameEntityId entityId)
        {
            return Check(entityId, Vector2.left, wantNormalXPositive: true);
        }

        public bool IsBlockedRight(GameEntityId entityId)
        {
            return Check(entityId, Vector2.right, wantNormalXPositive: false);
        }

        private bool Check(GameEntityId entityId, Vector2 dir, bool wantNormalXPositive)
        {
            if (!_bodies.TryGet(entityId, out var body))
                return false;

            var u = body as Rigidbody2DPhysicsBody;
            if (u == null || u.Collider == null)
                return false;

            var self = u.Collider;
            var b = self.bounds;

            // Центр по Y, ближе к середине тела; по X — у грани коллайдера
            float y = b.center.y;

            float halfH = b.extents.y * (1f - HeightShrink);
            if (halfH < 0.02f) halfH = 0.02f;

            // тонкая вертикальная “плашка”
            var size = new Vector2(ProbeThickness, halfH * 2f);

            // origin на левой/правой границе + skin внутрь
            float x = dir.x < 0f
                ? (b.min.x + Skin)
                : (b.max.x - Skin);

            var origin = new Vector2(x, y);

            int count = Physics2D.BoxCast(origin, size, 0f, dir, _filter, _hits, CheckDistance);

            for (int i = 0; i < count; i++)
            {
                var hit = _hits[i];
                var c = hit.collider;

                if (c == null || c == self)
                    continue;

                // Нормаль стены:
                // когда мы кастим ВЛЕВО, стена “слева” даст нормаль вправо (+x)
                // когда кастим ВПРАВО, стена “справа” даст нормаль влево (-x)
                var nx = hit.normal.x;

                if (wantNormalXPositive)
                {
                    if (nx >= MinWallNormalAbsX) return true;
                }
                else
                {
                    if (nx <= -MinWallNormalAbsX) return true;
                }
            }

            return false;
        }
    }
}
