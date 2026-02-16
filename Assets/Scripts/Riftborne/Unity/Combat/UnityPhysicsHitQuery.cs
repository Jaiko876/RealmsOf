using System.Collections.Generic;
using Riftborne.Core.Combat.Config;
using Riftborne.Core.Combat.Resolution;
using Riftborne.Core.Model;
using Riftborne.Core.Physics.Abstractions;
using Riftborne.Physics.Unity2D;
using UnityEngine;

namespace Riftborne.Unity.Combat
{
    public sealed class UnityPhysicsHitQuery : IHitQuery
    {
        private readonly IBodyProvider<GameEntityId> _bodies;
        private readonly HitQueryTuning _tuning;

        private readonly int _hitMask;

        public UnityPhysicsHitQuery(IBodyProvider<GameEntityId> bodies, HitQueryTuning tuning)
        {
            _bodies = bodies;
            _tuning = tuning;
            _hitMask = tuning.HitMask;
        }

        public void QueryHits(GameEntityId attacker, List<GameEntityId> results)
        {
            if (!_bodies.TryGet(attacker, out var body))
                return;

            var center = new Vector2(body.X, body.Y);

            float dir = body.Vx >= 0f ? 1f : -1f;
            center.x += dir * _tuning.ForwardOffset;

            var colliders = Physics2D.OverlapBoxAll(
                center,
                new Vector2(_tuning.HitWidth, _tuning.HitHeight),
                0f,
                _hitMask);

            if (colliders == null || colliders.Length == 0)
                return;

            for (int i = 0; i < colliders.Length; i++)
            {
                var col = colliders[i];
                if (col == null)
                    continue;

                var authoring = col.GetComponent<PhysicsBodyAuthoring>();
                if (authoring == null)
                    continue;

                var targetId = authoring.EntityId;
                if (targetId.Equals(attacker))
                    continue;

                results.Add(targetId);
            }
        }
    }
}
