using System;
using System.Collections.Generic;
using Gameplay.Agents;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Gameplay.Items
{
    /// <summary>
    /// Represents an interactive torch item
    /// within the level. Can be lit by a nearby player
    /// and extinguished by enemies. Visually updates
    /// its appearance and has a burning animation.
    /// Notifies the game of state changes.
    /// </summary>
    public class Torch : MonoBehaviour
    {
        // Animator parameter hash for the "IsLit" boolean
        private static readonly int IsLit = Animator.StringToHash("IsLit");
        
        // Fired once when the torch becomes lit by a player
        public event Action OnLit;
        
        // Fired whenever the torch's lit state changes
        public static event Action<bool> OnStateChanged;
        public bool IsTorchLit { get; private set; }

        [SerializeField] private Animator animator;
        [SerializeField] private Renderer lit;
        [SerializeField] private Renderer unlit;
        [SerializeField] private Light2D torchLight;
        [SerializeField] private Canvas feedback;

        [Header("Light Flicker Settings")]
        [SerializeField] private float intensityMin;
        [SerializeField] private float intensityMax;
        [SerializeField] private float radiusMin;
        [SerializeField] private float radiusMax;
        [SerializeField] private float flickerSpeed;

        // Tracks the currently selected player agent for interaction
        private PlayerAgent _currentSelected;
        
        // Collection of players currently within interaction range of this torch
        private HashSet<PlayerAgent> _nearbyPlayers = new();

        private void Start()
        {
            SetLit(false, false);
        }

        private void Awake()
        {
            InputController.OnInteractPressed += TryPlayerToggle;
            InputController.OnPlayerAgentClicked += HandleSelectionChanged;
        }

        private void OnDestroy()
        {
            InputController.OnInteractPressed -= TryPlayerToggle;
            InputController.OnPlayerAgentClicked -= HandleSelectionChanged;
        }

        /// <summary>
        /// Ticker - updates the torch’s flickering light effect if lit.
        /// </summary>
        private void Update()
        {
            if (IsTorchLit)
            {
                // Use Perlin noise for smooth, natural flicker movement over time
                float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);
                
                // Interpolate light intensity and radius
                torchLight.intensity = Mathf.Lerp(intensityMin, intensityMax, noise);
                torchLight.pointLightOuterRadius = Mathf.Lerp(radiusMin, radiusMax, noise);
            }
        }

        /// <summary>
        /// Detects when a player or enemy enters the torch’s trigger area.
        /// Players are tracked for potential interaction.
        /// Enemies may attempt to extinguish the torch.
        /// </summary>
        /// <param name="other">The collider that entered the trigger.</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            Agent agent = other.GetComponent<Agent>();
            if (!agent) return;

            if (agent is PlayerAgent player)
            {
                _nearbyPlayers.Add(player);
                UpdateFeedback();
            }
            else if (agent is EnemyAgent enemyAgent && IsTorchLit)
                // Enemies have a random chance to extinguish the torch if it's lit
                if (UnityEngine.Random.value < enemyAgent.ExtinguishTorchChance)
                    SetLit(false);
        }

        /// <summary>
        /// Detects when a player leaves the torch’s trigger area
        /// and removes them from tracking.
        /// </summary>
        /// <param name="other">The collider that exited the trigger.</param>
        private void OnTriggerExit2D(Collider2D other)
        {
            PlayerAgent player = other.GetComponent<PlayerAgent>();
            if (player && _nearbyPlayers.Remove(player))
                UpdateFeedback();
        }

        /// <summary>
        /// Attempts to light the torch when the player presses the interact button.
        /// Only the currently selected player, if nearby and while the torch is unlit,
        /// can successfully light the torch.
        /// </summary>
        private void TryPlayerToggle()
        {
            // Torch alredy lit, ignore
            if (IsTorchLit) return;
            
            // Only allow the selected player within range to light the torch
            if (_currentSelected && _nearbyPlayers.Contains(_currentSelected))
                SetLit(true);
        }

        /// <summary>
        /// Handles changes to the player agent selection.
        /// Updates feedback visibility based on the new selection.
        /// </summary>
        /// <param name="newSelection">The newly selected player agent.</param>
        private void HandleSelectionChanged(PlayerAgent newSelection)
        {
            _currentSelected = newSelection;
            UpdateFeedback();
        }

        /// <summary>
        /// Updates the torch’s state to lit or unlit and triggers related effects.
        /// </summary>
        /// <param name="litState">True to light the torch; false to extinguish it.</param>
        /// <param name="invoke">If true, relevant events are fired. Defaults to true.</param>
        private void SetLit(bool litState, bool invoke = true)
        {
            IsTorchLit = litState;

            if (invoke)
                OnStateChanged?.Invoke(IsTorchLit);

            if (IsTorchLit) 
                OnLit?.Invoke();

            // Update animator and visual/light components to match the new state
            animator.SetBool(IsLit, IsTorchLit);
            torchLight.enabled = IsTorchLit;
            lit.enabled = IsTorchLit;
            unlit.enabled = !IsTorchLit;

            UpdateFeedback();
        }

        /// <summary>
        /// Updates the interaction feedback UI.
        /// Displays a prompt only when the selected player
        /// is nearby and the torch is unlit.
        /// </summary>
        private void UpdateFeedback()
        {
            bool show = !IsTorchLit &&
                        _currentSelected &&
                        _nearbyPlayers.Contains(_currentSelected);

            feedback.gameObject.SetActive(show);
        }
    }
}
