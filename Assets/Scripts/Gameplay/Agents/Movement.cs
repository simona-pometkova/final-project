using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Gameplay.Agents
{
    /// <summary>
    /// Static class responsible for agent
    /// movement behaviour logic.
    /// </summary>
    public static class Movement
    {
        public static readonly LayerMask WallMask = 1 << 3;
        public static readonly float WallCheckDistance = 1.5f;

        /// <summary>
        /// Up, down, left, right and diagonal directions.
        /// </summary>
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

        /// <summary>
        /// Moves the rigidbody.
        /// </summary>
        /// <param name="rb">The rigidbody to move.</param>
        /// <param name="direction">The direction to move in.</param>
        /// <param name="speed">The movement speed.</param>
        public static void MoveRigidbody(Rigidbody2D rb, Vector2 direction, float speed)
        {
            rb.linearVelocity = direction * speed;
        }
        
        /// <summary>
        /// Determines a random direction to move in
        /// from all available movement directions.
        /// </summary>
        /// <param name="agent">The agent to set the new movement direction of.</param>
        /// <returns>A direction to move in.</returns>
        public static Vector2 PickRandomDirection(Agent agent)
        {
            Vector2 direction;
            
            // Idle check - don't move
            if (Random.value < agent.IdleChance)
                direction = Vector2.zero;
            else
            {
                var validDirections = new List<Vector2>();

                // Loop over all directions
                foreach (var dir in MovementDirections)
                {
                    // No walls in the way
                    RaycastHit2D hit = Physics2D.Raycast(agent.transform.position, dir, WallCheckDistance, WallMask);
                    if (!hit.collider) 
                        validDirections.Add(dir);
                }

                // Choose a random direction from all valid ones
                if (validDirections.Count > 0)
                    direction = validDirections[Maths.GetRandomInt(0, validDirections.Count)];
                else
                    // No free directions, stay idle
                    direction = Vector2.zero;
            }

            return direction;
        }
        
        /// <summary>
        /// Get direction determined by player input.
        /// </summary>
        /// <returns>A direction to move in.</returns>
        public static Vector2 GetInputDirection()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");

            return new Vector2(moveX, moveY).normalized;
        }
    }
}