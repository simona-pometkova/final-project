using System;
using UnityEngine;

namespace Gameplay.Agents
{
    public class PlayerAgent : Agent 
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;

        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");

            Vector2 moveDir = new Vector2(moveX, moveY).normalized;

            _rb.linearVelocity = moveDir * moveSpeed;
        }
    }
}