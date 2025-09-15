using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Agents
{
    // TODO documentation
    public abstract class Agent : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] protected float moveSpeed = 5f;
        [SerializeField] protected float minDirectionChangeTime = 0.5f;
        [SerializeField] protected float maxDirectionChangeTime = 3f;
        [SerializeField] protected Rigidbody2D rb;
        [SerializeField] private float idleChance = 0.2f;

        protected Vector2 _currentDirection;
        private float _timer;
        private bool _hasConverted;

        public static event Action<Agent> OnAgentCollision;
        
        protected virtual void Start()
        {
            _currentDirection = Movement.PickRandomDirection(this, idleChance);
        }

        protected virtual void Update()
        {
            _timer -= Time.deltaTime;
            
            if (_timer <= 0f)
                SetNewDirection();

            if (_currentDirection != Vector2.zero)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, _currentDirection, Movement.WallCheckDistance, Movement.WallMask);
                if (hit.collider)
                    SetNewDirection();
            }

            Movement.MoveRigidbody(rb, _currentDirection, moveSpeed);
        }

        private void SetNewDirection()
        {
            _currentDirection = Movement.PickRandomDirection(this, idleChance);
            _timer = Random.Range(minDirectionChangeTime, maxDirectionChangeTime);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            var agent = other.gameObject.GetComponent<Agent>();

            if (agent)
            {
                if (this is PlayerAgent && agent is EnemyAgent
                    || this is EnemyAgent && agent is PlayerAgent)
                {
                    if (!_hasConverted)
                    {
                        OnAgentCollision?.Invoke(this);
                        _hasConverted = true;
                    }
                }
            }
        }
    }
}

