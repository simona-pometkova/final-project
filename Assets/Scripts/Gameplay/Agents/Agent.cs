using System;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Gameplay.Agents
{
    /// <summary>
    /// Base class that implements the common behaviour
    /// of enemies and players throughout the dungeon.
    /// </summary>
    public abstract class Agent : MonoBehaviour
    {
        // State
        public bool HasConverted { get; private set; }
        public float IdleChance => idleChance;
        public static event Action<Agent, Agent> OnAgentCollision;

        [SerializeField] protected Rigidbody2D rb;

        // Movement stats - randomized via Scriptable Object data
        [Header("Movement")]
        [SerializeField] protected float moveSpeed;
        [SerializeField] protected float minDirectionChangeTime;
        [SerializeField] protected float maxDirectionChangeTime;
        [SerializeField] protected float idleChance;

        protected Vector2 _currentDirection;
        
        // Determines when to change the agent's movement direction
        private float _timer;

        /// <summary>
        /// Sets randomized movement stats
        /// and initializes the agent's behaviour.
        /// </summary>
        /// <param name="data">The Scriptable Object data class to use for randomization.</param>
        public virtual void Initialize(AgentData data)
        {
            data.RandomizeMovementStats(out moveSpeed, 
                out minDirectionChangeTime,
                out maxDirectionChangeTime, 
                out idleChance);

            SetNewDirection();
        }
        
        /// <summary>
        /// Ticker - common agent random walk behaviour.
        /// </summary>
        protected virtual void Update()
        {
            _timer -= Time.deltaTime;
            
            // When timer runs out, pick a new direction
            if (_timer <= 0f)
                SetNewDirection();
            
            // Unless the agent is currently idle,
            // check if any walls are in the way - if so, change direction
            if (_currentDirection != Vector2.zero) 
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, _currentDirection, Movement.WallCheckDistance, Movement.WallMask);
                if (hit.collider)
                    SetNewDirection();
            }

            Movement.MoveRigidbody(rb, _currentDirection, moveSpeed);
        }

        /// <summary>
        /// Determine whether to stay idle or pick a new direction to move in.
        /// </summary>
        private void SetNewDirection()
        {
            _currentDirection = Movement.PickRandomDirection(this);
            _timer = Maths.GetRandomFloat(minDirectionChangeTime, maxDirectionChangeTime);
        }

        /// <summary>
        /// Unity event - used to signal collision between two agents.
        /// </summary>
        /// <param name="other">The body this agent is colliding with.</param>
        private void OnCollisionEnter2D(Collision2D other)
        {
            // Convert once per current collision
            if (HasConverted) return;

            Agent otherAgent = other.gameObject.GetComponent<Agent>();

            if (!otherAgent || otherAgent.HasConverted) return;

            // Determine whether the collision is happening between opponents
            bool crossTeam = (this is PlayerAgent && otherAgent is EnemyAgent) ||
                             (this is EnemyAgent && otherAgent is PlayerAgent);

            if (crossTeam)
                // Trigger collision event
                OnAgentCollision?.Invoke(this, otherAgent);
        }

        /// <summary>
        /// Change the agent's converted state.
        /// </summary>
        /// <param name="converted">True if the agent has been converted.</param>
        public void SetConverted(bool converted) => HasConverted = converted;
    }
}

