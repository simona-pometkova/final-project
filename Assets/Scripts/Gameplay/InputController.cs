using System;
using Gameplay.Agents;
using UnityEngine;

namespace Gameplay
{
    public class InputController : MonoBehaviour
    {
        public static event Action<PlayerAgent> OnPlayerAgentClicked;
        public static event Action<bool> OnInteractPressed;

        [SerializeField] private LayerMask playerAgentLayer;

        private PlayerAgent _currentlySelectedAgent;

        private void Update()
        {
            TrackMouse();
            TrackKeys();
        }

        private void TrackMouse()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 worldPosition = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D hit = Physics2D.OverlapPoint(worldPosition, playerAgentLayer);

                if (hit != null)
                {
                    PlayerAgent agent = hit.GetComponent<PlayerAgent>();
                    if (agent != null && agent != _currentlySelectedAgent)
                    {
                        _currentlySelectedAgent?.ToggleSelected(false);
                        _currentlySelectedAgent = agent;
                        _currentlySelectedAgent.ToggleSelected(true);
                        OnPlayerAgentClicked?.Invoke(agent);
                        
                    }
                }
            }
        }

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
                OnInteractPressed?.Invoke(true);
        }
    }
}