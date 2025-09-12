using UnityEngine;

namespace Gameplay.Agents
{
    public class PlayerAgent : Agent 
    {
        public bool IsCurrentlySelected { get; private set; }
        public void ToggleSelected(bool selected) => IsCurrentlySelected = selected;

        protected override void Update()
        {
            if (IsCurrentlySelected)
                HandleMovement();
            else
                base.Update();
        }

        // TODO extract movement logic
        private void HandleMovement()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");

            Vector2 moveDir = new Vector2(moveX, moveY).normalized;

            _rb.linearVelocity = moveDir * moveSpeed;
        }
    }
}