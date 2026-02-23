using System;
using Riftborne.Core.Config;
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

        private readonly PhysicsProbesTuning.GroundProbeTuning _tuning;

        public Unity2DGroundSensor(IBodyProvider<GameEntityId> bodies, IGameplayTuning tuning)
        {
            if (bodies == null) throw new ArgumentNullException(nameof(bodies));
            if (tuning == null) throw new ArgumentNullException(nameof(tuning));

            _bodies = bodies;
            _tuning = tuning.PhysicsProbes.Ground;

            _filter = new ContactFilter2D();
            _filter.useLayerMask = true;
            _filter.layerMask = tuning.PhysicsProbes.Ground.GroundLayerMask;
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

            var origin = new Vector2(b.center.x, b.min.y + _tuning.Skin);

            var widthMul = _tuning.WidthMultiplier;
            if (widthMul < 0.05f) widthMul = 0.05f;
            if (widthMul > 1.00f) widthMul = 1.00f;

            var probeH = _tuning.ProbeHeight;
            if (probeH < 0.001f) probeH = 0.001f;

            var size = new Vector2(b.size.x * widthMul, probeH);

            int count = Physics2D.BoxCast(origin, size, 0f, Vector2.down, _filter, _hits, _tuning.CheckDepth);
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