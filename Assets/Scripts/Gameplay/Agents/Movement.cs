using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Gameplay.Agents
{
    // TODO documentation
    public static class Movement
    {
        public static readonly LayerMask WallMask = 1 << 3;
        public const float WallCheckDistance = 1.5f;
        
        private static readonly Vector2[] MovementDirections =
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

        public static void MoveRigidbody(Rigidbody2D rb, Vector2 direction, float speed)
        {
            rb.linearVelocity = direction * speed;
        }
        
        public static Vector2 PickRandomDirection(Agent agent, float idleChance)
        {
            Vector2 direction;
            
            // Idle check first
            if (Random.value < idleChance)
            {
                direction = Vector2.zero;
            }
            else
            {
                // Build a list of valid directions
                var validDirections = new List<Vector2>();

                foreach (var dir in MovementDirections)
                {
                    RaycastHit2D hit = Physics2D.Raycast(agent.transform.position, dir, WallCheckDistance, WallMask);
                    if (!hit.collider) 
                        validDirections.Add(dir);
                }

                if (validDirections.Count > 0)
                    direction = validDirections[Maths.GetRandomInt(0, validDirections.Count)];
                else
                    // No free directions, stay idle
                    direction = Vector2.zero;
            }

            return direction;
        }
        
        public static Vector2 GetInputDirection()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");

            return new Vector2(moveX, moveY).normalized;
        }
    }
}