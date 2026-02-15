using System.Collections.Generic;
using Game.Core.Combat.Resolution;
using Game.Core.Model;
using Game.Core.Physics.Abstractions;
using Game.Physics.Unity2D;
using UnityEngine;

namespace Game.Unity.Combat
{
    public sealed class UnityPhysicsHitQuery : IHitQuery
    {
        private readonly IBodyProvider<GameEntityId> _bodies;

        private const float HitWidth = 1.0f;
        private const float HitHeight = 0.8f;
        private const float ForwardOffset = 0.6f;

        private readonly LayerMask _hitMask;

        public UnityPhysicsHitQuery(IBodyProvider<GameEntityId> bodies)
        {
            _bodies = bodies;
            _hitMask = LayerMask.GetMask("Player", "Enemy");
        }

        public void QueryHits(GameEntityId attacker, List<GameEntityId> results)
        {
            if (!_bodies.TryGet(attacker, out var body))
                return;

            var center = new Vector2(body.X, body.Y);

            // пока берём направление из скорости
            float dir = body.Vx >= 0f ? 1f : -1f;
            center.x += dir * ForwardOffset;

            var colliders = Physics2D.OverlapBoxAll(
                center,
                new Vector2(HitWidth, HitHeight),
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
