using Riftborne.Core.Model;
using UnityEngine;

namespace Riftborne.Unity.View.Registry
{
    public interface IEntityViewRegistry
    {
        void RegisterFollowTarget(GameEntityId id, Transform target);
        void UnregisterFollowTarget(GameEntityId id, Transform target);
        bool TryGetFollowTarget(GameEntityId id, out Transform target);
    }
}