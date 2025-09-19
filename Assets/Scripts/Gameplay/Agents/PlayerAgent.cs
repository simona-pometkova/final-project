using UnityEngine;

namespace Gameplay.Agents
{
    /// <summary>
    /// Child class deriving from base Agent.
    /// Implements player-specific behavior.
    /// </summary>
    public class PlayerAgent : Agent
    {
        
        // State
        private bool _isCurrentlySelected;

        /// <summary>
        /// Ticker - user control if selected
        /// or fallback to base random walk behaviour.
        /// </summary>
        protected override void Update()
        {
            if (_isCurrentlySelected)
                HandleMovement();
            else
                base.Update();
        }

        /// <summary>
        /// Set state.
        /// </summary>
        /// <param name="selected">True if currently selected.</param>
        public void ToggleSelected(bool selected) => _isCurrentlySelected = selected;

        /// <summary>
        /// Handle movement via player input.
        /// </summary>
        private void HandleMovement()
        {
            Vector2 moveDir = Movement.GetInputDirection();
            Movement.MoveRigidbody(rb, moveDir, moveSpeed);
        }
    }
}