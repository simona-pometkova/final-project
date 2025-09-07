using UnityEngine;

namespace Gameplay.Agents
{
    public abstract class Agent : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] protected float moveSpeed = 5f;

        protected Rigidbody2D _rb;

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        protected virtual void Update()
        {
            Move();
        }

        // TODO random walk algorithm/logic
        private void Move()
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;

            _rb.linearVelocity = randomDir * moveSpeed;
        }
    }
}

