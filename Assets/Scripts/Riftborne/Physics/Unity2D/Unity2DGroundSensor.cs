using Riftborne.Core.Model;
using Riftborne.Core.Physics.Abstractions;
using UnityEngine;

namespace Riftborne.Physics.Unity2D
{
    public sealed class Unity2DGroundSensor : IGroundSensor
    {
        private readonly IBodyProvider<GameEntityId> _bodies;
        private readonly ContactFilter2D _filter;
        private readonly RaycastHit2D[] _hits = new RaycastHit2D[4];

        // Настройки “ног”
        private const float Skin = 0.02f;        // чуть внутрь коллайдера
        private const float CheckDepth = 0.08f;  // насколько вниз проверяем

        public Unity2DGroundSensor(IBodyProvider<GameEntityId> bodies)
        {
            _bodies = bodies;

            // Фильтр: только "Ground" слои, без триггеров
            _filter = new ContactFilter2D();
            _filter.useLayerMask = true;
            _filter.layerMask = LayerMask.GetMask("Ground"); // создай слой Ground и назначь его полу
            _filter.useTriggers = false;
        }

        public bool IsGrounded(GameEntityId entityId)
        {
            if (!_bodies.TryGet(entityId, out var body))
                return false;

            var u = body as Rigidbody2DPhysicsBody;
            if (u == null || u.Collider == null)
                return false;

            var b = u.Collider.bounds;

            // Центр “ног” — по x по центру, по y на нижней границе
            var origin = new Vector2(b.center.x, b.min.y + Skin);

            // Ширина бокса: чуть уже коллайдера (чтобы не цеплять стены)
            var size = new Vector2(b.size.x * 0.9f, 0.02f);

            // Cast вниз
            int count = Physics2D.BoxCast(origin, size, 0f, Vector2.down, _filter, _hits, CheckDepth);
            for (int i = 0; i < count; i++)
            {
                var c = _hits[i].collider;
                if (c != null && c != u.Collider)
                    return true;
            }
            return false;

        }
    }
}
