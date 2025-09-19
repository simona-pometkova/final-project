using Gameplay.Items;
using UnityEngine;

namespace Gameplay.Agents
{
    /// <summary>
    /// Child class deriving from base Agent.
    /// Implements enemy-specific behavior.
    /// </summary>
    public class EnemyAgent : Agent
    {
        public float ExtinguishTorchChance => extinguishTorchChance;
        
        // Enemy-specific stats - randomized via Scriptable Object data
        [Header("Behavior")]
        [SerializeField] private float extinguishTorchChance;
        [SerializeField] private float lookForTorchChance;
        [SerializeField] private float searchRadius;
        [SerializeField] private float torchCheckInterval;

        // Used to determine that the enemy has reached the target torch
        private const float ArrivalDistance = 1f;
        
        // The torch the enemy is currently heading towards
        private Torch _targetTorch;
        
        // Determines when to scan the area for torches
        private float _torchCheckTimer;

        /// <summary>
        /// Sets common agent and enemy-specific stats
        /// and initializes the enemy's behavior.
        /// </summary>
        /// <param name="data">The Scriptable Object data class to use for randomization.</param>
        public override void Initialize(AgentData data)
        {
            base.Initialize(data);
            data.RandomizeEnemyStats(out extinguishTorchChance,
                out lookForTorchChance,
                out torchCheckInterval,
                out searchRadius);
        }

        /// <summary>
        /// Ticker - find and move towards target torch
        /// or fallback to base random walk behaviour.
        /// </summary>
        protected override void Update()
        {
            _torchCheckTimer -= Time.deltaTime;

            // Find a new target torch
            if (_torchCheckTimer <= 0f)
            {
                _torchCheckTimer = torchCheckInterval;
                _targetTorch = Random.value < lookForTorchChance ? FindNearbyTorch() : null;
            }

            // Move towards target torch
            if (_targetTorch)
            {
                float distance = Vector2.Distance(transform.position, _targetTorch.transform.position);

                // Stop moving if we've arrived
                if (distance <= ArrivalDistance)
                {
                    _currentDirection = Vector2.zero;
                    Movement.MoveRigidbody(rb, Vector2.zero, 0f);

                    // Clear target so the enemy can resume wandering after extinguish
                    _targetTorch = null;
                    return;
                }

                // Otherwise keep moving toward the torch
                _currentDirection = (_targetTorch.transform.position - transform.position).normalized;
                Movement.MoveRigidbody(rb, _currentDirection, moveSpeed);
            }
            else
                // Fallback to normal wandering
                base.Update();
        }

        /// <summary>
        /// Scans the area for torches and tries to find a valid target.
        /// </summary>
        /// <returns>A valid nearby torch or null.</returns>
        private Torch FindNearbyTorch()
        {
            Torch target = null;
            float closest = float.MaxValue;

            // Loop through all torches
            foreach (var torch in FindObjectsByType<Torch>(FindObjectsSortMode.None))
            {
                // Skip already extinguished torches
                if (!torch.IsTorchLit) continue;

                // Find the closest torch
                float distance = Vector2.Distance(transform.position, torch.transform.position);
                if (distance < closest && distance <= searchRadius)
                {
                    // Check line of sight (no walls between enemy and torch)
                    if (!Physics2D.Raycast(transform.position,
                                           (torch.transform.position - transform.position).normalized,
                                           distance,
                                           Movement.WallMask))
                    {
                        closest = distance;
                        target = torch;
                    }
                }
            }
            return target;
        }
    }
}