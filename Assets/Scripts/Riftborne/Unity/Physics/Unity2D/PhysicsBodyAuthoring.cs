using System;
using Riftborne.Core.Entities;
using Riftborne.Core.Model;
using Riftborne.Core.Physics.Abstractions;
using UnityEngine;
using VContainer;

namespace Riftborne.Unity.Physics.Unity2D
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PhysicsBodyAuthoring : MonoBehaviour, IGameEntityIdReceiver
    {
        [Header("Scene fallback (debug only)")]
        [SerializeField] private int sceneEntityId = 0;

        private GameEntityId _id;
        private bool _hasId;

        private bool _registered;

        private IBodyProvider<GameEntityId> _registry;
        private Rigidbody2DPhysicsBody _body;

        public GameEntityId EntityId { get { return _id; } }

        private void Awake()
        {
            _body = new Rigidbody2DPhysicsBody(GetComponent<Rigidbody2D>());
        }

        [Inject]
        public void Construct(IBodyProvider<GameEntityId> registry)
        {
            _registry = registry;
            TryRegister();
        }

        public void SetEntityId(GameEntityId id)
        {
            if (_registered)
                throw new InvalidOperationException("Cannot change EntityId after registration.");

            _id = id;
            _hasId = true;
            sceneEntityId = id.Value;

            TryRegister();
        }

        private void OnEnable()
        {
            // Scene fallback (hand-placed debug objects)
            if (!_hasId)
            {
                _id = new GameEntityId(sceneEntityId);
                _hasId = true;
            }

            TryRegister();
        }

        private void OnDisable()
        {
            if (_registered && _registry != null)
            {
                _registry.Unregister(_id);
                _registered = false;
            }
        }

        private void TryRegister()
        {
            if (_registered) return;
            if (!_hasId) return;
            if (_registry == null || _body == null) return;

            _registry.Register(_id, _body);
            _registered = true;
        }
    }
}