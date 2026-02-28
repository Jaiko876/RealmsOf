using System;
using System.Collections.Generic;
using Riftborne.Core.Entities;
using Riftborne.Core.Model;
using UnityEngine;

namespace Riftborne.Unity.Entities
{
    [DefaultExecutionOrder(-10000)]
    [DisallowMultipleComponent]
    public sealed class EntityIdentityAuthoring : MonoBehaviour
    {
        [Header("Scene fallback (for hand-placed objects only)")]
        [SerializeField] private int sceneEntityId = 0;
        [SerializeField] private int sceneOwnerPlayerId = -1;

        private GameEntityId _entityId;
        private bool _hasEntityId;

        private PlayerId _owner;
        private bool _hasOwner;

        private bool _applied;

        private readonly List<MonoBehaviour> _behaviours = new List<MonoBehaviour>(32);

        public bool HasEntityId { get { return _hasEntityId; } }
        public GameEntityId EntityId { get { return _entityId; } }

        public bool HasOwner { get { return _hasOwner; } }
        public PlayerId Owner { get { return _owner; } }

        /// <summary>
        /// Assign runtime identity BEFORE enabling the instance in hierarchy.
        /// Can be called on inactive objects.
        /// </summary>
        public void AssignEntityId(GameEntityId id)
        {
            if (_applied)
                throw new InvalidOperationException("EntityIdentityAuthoring: cannot assign EntityId after it was applied.");

            _entityId = id;
            _hasEntityId = true;
        }

        /// <summary>
        /// Optional owner binding (PlayerId) for player avatars.
        /// </summary>
        public void AssignOwner(PlayerId playerId)
        {
            if (_applied)
                throw new InvalidOperationException("EntityIdentityAuthoring: cannot assign Owner after it was applied.");

            _owner = playerId;
            _hasOwner = true;
        }

        private void OnEnable()
        {
            ApplyIfNeeded();
        }

        private void ApplyIfNeeded()
        {
            if (_applied)
                return;

            if (!_hasEntityId)
            {
                _entityId = new GameEntityId(sceneEntityId);
                _hasEntityId = true;
            }

            if (!_hasOwner && sceneOwnerPlayerId >= 0)
            {
                _owner = new PlayerId(sceneOwnerPlayerId);
                _hasOwner = true;
            }

            Broadcast(_entityId, _hasOwner ? (PlayerId?)_owner : null);

            _applied = true;
        }

        private void Broadcast(GameEntityId entityId, PlayerId? owner)
        {
            _behaviours.Clear();
            GetComponentsInChildren(true, _behaviours);

            for (int i = 0; i < _behaviours.Count; i++)
            {
                var b = _behaviours[i];
                if (b == null) continue;
                if (ReferenceEquals(b, this)) continue;

                var idRx = b as IGameEntityIdReceiver;
                if (idRx != null)
                    idRx.SetEntityId(entityId);

                if (owner.HasValue)
                {
                    var ownerRx = b as IPlayerOwnerReceiver;
                    if (ownerRx != null)
                        ownerRx.SetOwner(owner.Value);
                }
            }
        }
    }
}