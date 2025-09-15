using UnityEngine;

namespace Gameplay.Agents
{
    // TODO documentation
    public class PlayerAgent : Agent
    {
        private bool _isCurrentlySelected;
        public void ToggleSelected(bool selected) => _isCurrentlySelected = selected;
        protected override void Update()
        {
            if (_isCurrentlySelected)
                HandleMovement();
            else
                base.Update();
        }

        private void HandleMovement()
        {
            Vector2 moveDir = Movement.GetInputDirection();
            Movement.MoveRigidbody(rb, moveDir, moveSpeed);
        }
    }
}