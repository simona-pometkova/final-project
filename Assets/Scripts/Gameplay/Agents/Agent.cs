using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Agents
{
    // TODO documentation
    public abstract class Agent : MonoBehaviour
    {
        public bool HasConverted { get; private set; }

        public static event Action<Agent, Agent> OnAgentCollision;

        [SerializeField] protected Rigidbody2D rb;

        [Header("Movement")]
        [SerializeField] protected float moveSpeed;
        [SerializeField] protected float minDirectionChangeTime;
        [SerializeField] protected float maxDirectionChangeTime;
        [SerializeField] protected float idleChance;

        protected Vector2 _currentDirection;
        private float _timer;

        public virtual void Initialize(AgentData data)
        {
            data.RandomizeMovementStats(out moveSpeed, 
                out minDirectionChangeTime,
                out maxDirectionChangeTime, 
                out idleChance);

            SetNewDirection();
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
            if (HasConverted) return;

            Agent otherAgent = other.gameObject.GetComponent<Agent>();

            if (!otherAgent || otherAgent.HasConverted) return;

            bool crossTeam = (this is PlayerAgent && otherAgent is EnemyAgent) ||
                             (this is EnemyAgent && otherAgent is PlayerAgent);

            if (crossTeam)
                OnAgentCollision?.Invoke(this, otherAgent);
        }

        public void SetConverted(bool converted)
        {
            HasConverted = converted;
        }
    }
}

