using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Gameplay.Agents
{
    // TODO refactor, optimize. WIP
    public abstract class Agent : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] protected float moveSpeed = 5f;
        [SerializeField] protected float minDirectionChangeTime = 0.5f;
        [SerializeField] protected float maxDirectionChangeTime = 3f;

        [SerializeField] protected LayerMask wallMask;
        // [SerializeField] protected LayerMask agentMask;
        
        protected const float IdleChance = 0.2f;

        // private LayerMask obstacleMask;
        protected Rigidbody2D _rb;
        protected Vector2 _currentDirection;
        protected float _timer;
        protected float _wallCheckDistance = 1.5f;

        protected static readonly Vector2[] MovementDirections =
        {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right,
            (Vector2.up + Vector2.right).normalized,
            (Vector2.down + Vector2.right).normalized,
            (Vector2.down + Vector2.left).normalized,
            (Vector2.up + Vector2.left).normalized,
        };

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            // obstacleMask = wallMask | agentMask;
        }

        protected virtual void Start()
        {
            PickDirection();
        }

        protected virtual void Update()
        {
            _timer -= Time.deltaTime;
            
            if (_timer <= 0f)
                PickDirection();

            if (_currentDirection != Vector2.zero)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, _currentDirection, _wallCheckDistance, wallMask);
                if (hit.collider != null /*&& hit.collider.gameObject != gameObject*/)
                    PickDirection();
            }

            _rb.linearVelocity = _currentDirection * moveSpeed;
        }

        private void PickDirection()
        {
            // if (Random.value < IdleChance)
            //     _currentDirection = Vector2.zero;
            // else
            //     _currentDirection = MovementDirections[Random.Range(0, MovementDirections.Length)];
            //
            // _timer = Random.Range(minDirectionChangeTime, maxDirectionChangeTime);
            
            // Idle check first
            if (Random.value < IdleChance)
            {
                _currentDirection = Vector2.zero;
            }
            else
            {
                // Build a list of valid directions
                var validDirections = new System.Collections.Generic.List<Vector2>();

                foreach (var dir in MovementDirections)
                {
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, _wallCheckDistance, wallMask);
                    if (hit.collider == null /* || hit.collider.gameObject == gameObject */) // nothing blocking this direction
                    {
                        validDirections.Add(dir);
                    }
                }

                if (validDirections.Count > 0)
                {
                    _currentDirection = validDirections[Random.Range(0, validDirections.Count)];
                }
                else
                {
                    // No free directions, stay idle
                    _currentDirection = Vector2.zero;
                }
            }

            _timer = Random.Range(minDirectionChangeTime, maxDirectionChangeTime);
        }
    }
}

