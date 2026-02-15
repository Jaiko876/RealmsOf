using Game.Core.Model;
using Game.Core.Physics.Abstractions;
using UnityEngine;
using VContainer;

namespace Game.Physics.Unity2D
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PhysicsBodyAuthoring : MonoBehaviour
    {
        [SerializeField] private int entityId = 0;

        private GameEntityId _id;
        private bool _registered;

        private IBodyProvider<GameEntityId> _registry;
        private Rigidbody2DPhysicsBody _body;

        public GameEntityId EntityId
        {
            get { return _id; }
        }

        private void Awake()
        {
            _id = new GameEntityId(entityId);
            _body = new Rigidbody2DPhysicsBody(GetComponent<Rigidbody2D>());
        }

        [Inject]
        public void Construct(IBodyProvider<GameEntityId> registry)
        {
            _registry = registry;
            TryRegister();
        }

        private void OnEnable()
        {
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
            if (_registry == null || _body == null) return;

            _registry.Register(_id, _body);
            _registered = true;
        }

        /// <summary>
        /// На будущее для спавна: назначить id до регистрации.
        /// </summary>
        public void SetEntityId(GameEntityId id)
        {
            if (_registered)
            {
                // На старте лучше явно падать, чем молча ломать регистрацию.
                throw new System.InvalidOperationException("Cannot change EntityId after registration.");
            }

            _id = id;
            entityId = id.Value;
        }
    }
}
