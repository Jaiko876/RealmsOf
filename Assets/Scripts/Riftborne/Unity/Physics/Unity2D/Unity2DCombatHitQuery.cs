using Riftborne.Core.Gameplay.Combat.Abstractions;
using Riftborne.Core.Model;
using Riftborne.Unity.Entities;
using UnityEngine;

namespace Riftborne.Unity.Physics.Unity2D
{
    public sealed class Unity2DCombatHitQuery : ICombatHitQuery
    {
        private readonly Collider2D[] _hits;

        public Unity2DCombatHitQuery()
        {
            _hits = new Collider2D[32];
        }

        public int OverlapBox(float centerX, float centerY, float width, float height, int layerMask, GameEntityId[] results)
        {
            if (results == null || results.Length == 0)
                return 0;

            if (width <= 0f || height <= 0f)
                return 0;

            var center = new Vector2(centerX, centerY);
            var size = new Vector2(width, height);

            int count = Physics2D.OverlapBoxNonAlloc(center, size, 0f, _hits, layerMask);

            int w = 0;
            for (int i = 0; i < count && w < results.Length; i++)
            {
                var c = _hits[i];
                if (c == null) continue;

                if (TryGetEntityId(c, out var id))
                    results[w++] = id;
            }

            return w;
        }

        private static bool TryGetEntityId(Collider2D c, out GameEntityId id)
        {
            // Preferred: EntityIdentityAuthoring in hierarchy
            var identity = c.GetComponentInParent<EntityIdentityAuthoring>();
            if (identity != null && identity.HasEntityId)
            {
                id = identity.EntityId;
                return true;
            }

            // Fallback: Rigidbody with PhysicsBodyAuthoring may know id
            var rb = c.attachedRigidbody;
            if (rb != null)
            {
                var body = rb.GetComponent<PhysicsBodyAuthoring>();
                if (body != null)
                {
                    id = body.EntityId;
                    return true;
                }
            }

            id = default;
            return false;
        }
    }
}