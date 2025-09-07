using System;
using UnityEngine;

namespace Gameplay.Agents
{
    public class PlayerAgent : Agent 
    {
        public bool IsCurrentlySelected { get; private set; }

        // TODO
        public static event Action<PlayerAgent> OnSelected;

        protected override void Awake()
        {
            base.Awake();
            OnSelected += Deselect;
        }

        protected override void Update()
        {
            if (IsCurrentlySelected)
                HandleMovement();
            else
                base.Update();
        }

        private void OnDisable()
        {
            OnSelected -= Deselect;
        }

        // TODO
        private void OnMouseDown()
        {
            IsCurrentlySelected = true;
            OnSelected?.Invoke(this);
        }

        private void HandleMovement()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");

            Vector2 moveDir = new Vector2(moveX, moveY).normalized;

            _rb.linearVelocity = moveDir * moveSpeed;
        }

        // TODO
        private void Deselect(PlayerAgent agent)
        {
            if (agent != this)
                IsCurrentlySelected = false;
        }
    }
}