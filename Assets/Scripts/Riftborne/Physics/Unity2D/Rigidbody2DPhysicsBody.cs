using Riftborne.Core.Physics.Abstractions;
using UnityEngine;

namespace Riftborne.Physics.Unity2D
{
    public sealed class Rigidbody2DPhysicsBody : IPhysicsBody
    {
        private readonly Rigidbody2D _rb;

        // Unity-специфика — НЕ в интерфейсе Core
        public Rigidbody2D Rigidbody => _rb;
        public Collider2D Collider { get; }

        public Rigidbody2DPhysicsBody(Rigidbody2D rb)
        {
            _rb = rb;
            Collider = rb.GetComponent<Collider2D>();
        }

        public float X
        {
            get => _rb.position.x;
            set => _rb.position = new Vector2(value, _rb.position.y);
        }

        public float Y
        {
            get => _rb.position.y;
            set => _rb.position = new Vector2(_rb.position.x, value);
        }

        public float Vx
        {
            get => _rb.linearVelocity.x;
            set => _rb.linearVelocity = new Vector2(value, _rb.linearVelocity.y);
        }

        public float Vy
        {
            get => _rb.linearVelocity.y;
            set => _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, value);
        }

        public void AddImpulse(float ix, float iy)
        {
            _rb.AddForce(new Vector2(ix, iy), ForceMode2D.Impulse);
        }
    }
}
