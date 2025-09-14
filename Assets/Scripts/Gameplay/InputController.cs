using System;
using Gameplay.Agents;
using UnityEngine;

namespace Gameplay
{
    // TODO add documentation
    public class InputController : MonoBehaviour
    {
        public static event Action<PlayerAgent> OnPlayerAgentClicked;
        public static event Action OnInteractPressed;

        [SerializeField] private LayerMask playerAgentLayer;

        private PlayerAgent _currentlySelectedAgent;

        private void Update()
        {
            TrackMouse();
            TrackKeys();
        }

        // TODO this method is quite expensive. Is there a way to optimise it?
        private void TrackMouse()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 worldPosition = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D hit = Physics2D.OverlapPoint(worldPosition, playerAgentLayer);

                if (hit)
                {
                    PlayerAgent agent = hit.GetComponent<PlayerAgent>();
                    if (agent && agent != _currentlySelectedAgent)
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
                OnInteractPressed?.Invoke();
        }
    }
}