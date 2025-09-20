using System;
using Gameplay.Agents;
using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// Handles all player input for selecting agents
    /// and triggering global actions.
    /// Listens for mouse and keyboard input
    /// to select or deselect player agent objects,
    /// trigger interaction commands,
    /// and quit the current level.
    /// </summary>
    public class InputController : MonoBehaviour
    {
        // Fired when a PlayerAgent is clicked
        public static event Action<PlayerAgent> OnPlayerAgentClicked;
        
        // Fired when interact key ('E') is pressed
        public static event Action OnInteractPressed;
        
        // Fired when quit key ('Q') is pressed
        public static event Action OnQuitPressed;

        [SerializeField] private LayerMask playerAgentLayer;

        private PlayerAgent _currentlySelectedAgent;

        /// <summary>
        /// Ticker - checks for mouse and keyboard input each frame
        /// </summary>
        private void Update()
        {
            TrackMouse();
            TrackKeys();
        }

        /// <summary>
        /// Detects left mouse button clicks to select a PlayerAgent.
        /// If a new agent is clicked, the previously selected agent (if any)
        /// is deselected. 
        /// </summary>
        private void TrackMouse()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Convert to world coordinates
                Vector2 worldPosition = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
                
                // Check if a player agent is under the mouse position
                Collider2D hit = Physics2D.OverlapPoint(worldPosition, playerAgentLayer);

                if (hit)
                {
                    PlayerAgent agent = hit.GetComponent<PlayerAgent>();
                    
                    // Only proceed if the clicked agent is valid 
                    // and different from the currently selected one
                    if (agent && agent != _currentlySelectedAgent)
                    {
                        // Deselect the currently selected agent (if any)
                        _currentlySelectedAgent?.ToggleSelected(false);
                        
                        // Update new agent
                        _currentlySelectedAgent = agent;
                        _currentlySelectedAgent.ToggleSelected(true);
                        
                        OnPlayerAgentClicked?.Invoke(agent);
                        
                    }
                }
            }
        }

        /// <summary>
        /// Detects key presses for interaction ('E'), quitting ('Q'),
        /// and deselecting the current agent ('Esc').
        /// </summary>
        private void TrackKeys()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && _currentlySelectedAgent != null)
            {
                _currentlySelectedAgent.ToggleSelected(false);
                _currentlySelectedAgent = null;
                
                // "Free" the camera & player control
                OnPlayerAgentClicked?.Invoke(null);
            }
            
            if (Input.GetKeyDown(KeyCode.E)) 
                OnInteractPressed?.Invoke();

            if (Input.GetKeyDown(KeyCode.Q))
                OnQuitPressed?.Invoke();
        }
    }
}