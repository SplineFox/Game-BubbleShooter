using System;
using UnityEngine;

namespace BubbleShooter.HexGrids
{
    public class Bubble : MonoBehaviour
    {
        public event Action Collided;

        [SerializeField] private Collider2D _collider;
        [SerializeField] private Rigidbody2D _rigidbody;

        public void SetVelocity(Vector2 velocity)
        {
            _rigidbody.velocity = velocity;
        }

        public void ResetVelocity()
        {
            _rigidbody.velocity = Vector2.zero;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("GridCollision"))
            {
                Collided?.Invoke();
                return;
            }

            _rigidbody.velocity = Vector2.Reflect(_rigidbody.velocity, collision.contacts[0].normal);
        }
    }
}