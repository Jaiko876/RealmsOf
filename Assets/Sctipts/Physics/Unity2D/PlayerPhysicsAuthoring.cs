using Game.Core.Model;
using Game.Core.Physics.Abstractions;
using UnityEngine;
using VContainer;

namespace Game.Physics.Unity2D
{

    /// <summary>
    /// Attach to any GameObject with Rigidbody2D to opt-in the player into physics.
    /// (Later you can generalize to EntityId for any object.)
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PlayerPhysicsAuthoring : MonoBehaviour
    {
        [SerializeField] private int playerId = 0;

        private PlayerId _id;
        private bool _registered;

        private IPlayerBodyProvider _registry;
        private Rigidbody2D _rb;
        private Rigidbody2DPhysicsBody _body;

        private void Awake()
        {
            _id = new PlayerId(playerId);
            _rb = GetComponent<Rigidbody2D>();
            _body = new Rigidbody2DPhysicsBody(_rb);
        }

        [Inject]
        public void Construct(IPlayerBodyProvider registry)
        {
            _registry = registry;

            // если объект уже enabled, а OnEnable успел сработать раньше инжекта
            TryRegister();
        }

        private void OnEnable() => TryRegister();

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
    }
}
