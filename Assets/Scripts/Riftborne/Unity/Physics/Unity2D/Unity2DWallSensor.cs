using System;
using Riftborne.Core.Config;
using Riftborne.Core.Model;
using Riftborne.Core.Physics.Abstractions;
using UnityEngine;

namespace Riftborne.Unity.Physics.Unity2D
{
    public sealed class Unity2DWallSensor : IWallSensor
    {
        private readonly IBodyProvider<GameEntityId> _bodies;
        private readonly ContactFilter2D _filter;
        private readonly RaycastHit2D[] _hits = new RaycastHit2D[8];

        private readonly PhysicsProbesTuning.WallProbeTuning _tuning;

        public Unity2DWallSensor(IBodyProvider<GameEntityId> bodies, IGameplayTuning tuning)
        {
            if (bodies == null) throw new ArgumentNullException(nameof(bodies));
            if (tuning == null) throw new ArgumentNullException(nameof(tuning));

            _bodies = bodies;
            _tuning = tuning.PhysicsProbes.Wall;

            _filter = new ContactFilter2D();
            _filter.useLayerMask = true;
            _filter.layerMask = tuning.PhysicsProbes.Wall.WallLayerMask;
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

            float y = b.center.y;

            float halfH = b.extents.y * (1f - _tuning.HeightShrink);
            if (halfH < 0.02f) halfH = 0.02f;

            var size = new Vector2(_tuning.ProbeThickness, halfH * 2f);

            float x = dir.x < 0f
                ? (b.min.x + _tuning.Skin)
                : (b.max.x - _tuning.Skin);

            var origin = new Vector2(x, y);

            int count = Physics2D.BoxCast(origin, size, 0f, dir, _filter, _hits, _tuning.CheckDistance);

            for (int i = 0; i < count; i++)
            {
                var hit = _hits[i];
                var c = hit.collider;

                if (c == null || c == self)
                    continue;

                var nx = hit.normal.x;

                if (wantNormalXPositive)
                {
                    if (nx >= _tuning.MinWallNormalAbsX) return true;
                }
                else
                {
                    if (nx <= -_tuning.MinWallNormalAbsX) return true;
                }
            }

            return false;
        }
    }
}