using Gameplay.Agents;
using System;
using UnityEngine;

namespace Gameplay.Camera
{
    /// <summary>
    /// Controls an orthographic 2D camera for moving around the dungeon.
    /// Supports movement by mouse position near screen edges and zooming via mouse scroll wheel.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private float movementSpeed = 10f;
        [SerializeField] private float movementEdgeThreshold = 10f;
        [SerializeField] private float zoomSpeed = 5f;
        [SerializeField] private float minZoom = 5f;
        [SerializeField] private float maxZoom = 20f;

        private UnityEngine.Camera _camera;
        private PlayerAgent _targetAgent; // TODO

        private void Awake()
        {
            InputController.OnPlayerAgentClicked += FollowAgent;
        }

        private void Start()
        {
            _camera = UnityEngine.Camera.main;
        }

        private void OnDestroy()
        {
            InputController.OnPlayerAgentClicked -= FollowAgent;
        }

        private void FollowAgent(PlayerAgent agent) => _targetAgent = agent;

        /// <summary>
        /// Detects mouse position and scroll input every frame to move and zoom the camera accordingly.
        /// Decides between free camera or tracking camera movement,
        /// depending on whether the player is controlling an agent.
        /// </summary>
        private void Update()
        {
            Zoom();
            
            if (_targetAgent != null)
                TrackAgent();
            else 
                FreeCameraMovement();
        }

        // TODO documentation
        private void Zoom()
        {
            // Zoom camera with mouse scroll wheel
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (Math.Abs(scroll) > 0.01f)
            {
                _camera.orthographicSize -= scroll * zoomSpeed;

                // Constrain zoom amount
                _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, minZoom, maxZoom);
            }
        }

        // TODO documentation
        private void TrackAgent()
        {
            Vector3 targetPosition = _targetAgent.transform.position;
            targetPosition.z = transform.position.z;
            transform.position = Vector3.Lerp(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        }

        // TODO documentation
        private void FreeCameraMovement()
        {
            // Camera movement
            Vector3 movement = Vector3.zero;
            Vector3 mousePosition = Input.mousePosition;

            // Move camera horizontally if mouse near screen edges
            if (mousePosition.x >= Screen.width - movementEdgeThreshold)
                movement.x += 1;
            else if (mousePosition.x <= movementEdgeThreshold)
                movement.x -= 1;

            // Move camera vertically if mouse near screen edges
            if (mousePosition.y >= Screen.height - movementEdgeThreshold)
                movement.y += 1;
            else if (mousePosition.y <= movementEdgeThreshold)
                movement.y -= 1;

            movement.Normalize();
            transform.position += movement * (movementSpeed * Time.deltaTime);
        }
    }
}