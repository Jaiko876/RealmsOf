using System;
using System.Collections.Generic;
using Riftborne.Core.Model;
using UnityEngine;

namespace Riftborne.Unity.View.Registry
{
    public sealed class EntityViewRegistry : IEntityViewRegistry
    {
        private readonly Dictionary<GameEntityId, Transform> _follow = new Dictionary<GameEntityId, Transform>();

        public void RegisterFollowTarget(GameEntityId id, Transform target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            _follow[id] = target;
        }

        public void UnregisterFollowTarget(GameEntityId id, Transform target)
        {
            if (target == null) return;

            if (_follow.TryGetValue(id, out var cur) && ReferenceEquals(cur, target))
                _follow.Remove(id);
        }

        public bool TryGetFollowTarget(GameEntityId id, out Transform target)
        {
            return _follow.TryGetValue(id, out target) && target != null;
        }
    }
}