using Gameplay.Items;
using UnityEngine;

namespace Gameplay.Agents
{
    // TODO add documentation
    public class EnemyAgent : Agent
    {
        public float ExtinguishTorchChance => extinguishTorchChance;

        // Stats
        public float extinguishTorchChance;
        public float lookForTorchChance;
        public float searchRadius;
        public float torchCheckInterval;

        private float arrivalDistance = 1f;
        private Torch _targetTorch;
        private float _torchCheckTimer;

        public void InitializeEnemy(float extinguishTorchChance, float lookForTorchChance, float torchCheckInterval, float searchRadius)
        {
            this.extinguishTorchChance = extinguishTorchChance;
            this.lookForTorchChance = lookForTorchChance;
            this.torchCheckInterval = torchCheckInterval;
            this.searchRadius = searchRadius;
        }

        protected override void Update()
        {
            _torchCheckTimer -= Time.deltaTime;

            if (_torchCheckTimer <= 0f)
            {
                _torchCheckTimer = torchCheckInterval;

                _targetTorch = Random.value < lookForTorchChance ? FindNearbyTorch() : null;
            }

            if (_targetTorch)
            {
                float dist = Vector2.Distance(transform.position, _targetTorch.transform.position);

                // Stop moving if we've arrived
                if (dist <= arrivalDistance)
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
            {
                // fallback to normal wandering
                base.Update();
            }
        }

        private Torch FindNearbyTorch()
        {
            Torch target = null;
            float closest = float.MaxValue;

            foreach (var torch in FindObjectsByType<Torch>(FindObjectsSortMode.None))
            {
                if (!torch.IsTorchLit) continue;

                float dist = Vector2.Distance(transform.position, torch.transform.position);
                if (dist < closest && dist <= searchRadius)
                {
                    // Check line of sight (no walls between enemy and torch)
                    if (!Physics2D.Raycast(transform.position,
                                           (torch.transform.position - transform.position).normalized,
                                           dist,
                                           Movement.WallMask))
                    {
                        closest = dist;
                        target = torch;
                    }
                }
            }
            return target;
        }
    }
}