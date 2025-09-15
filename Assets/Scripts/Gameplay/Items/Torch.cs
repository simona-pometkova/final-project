using System;
using System.Collections.Generic;
using Gameplay.Agents;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Gameplay.Items
{
    // TODO add documentation
    public class Torch : MonoBehaviour
    {
        private static readonly int IsLit = Animator.StringToHash("IsLit");
        public event Action OnLit;
        public bool IsTorchLit => _isLit;
        
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

        private bool _isLit;
        private PlayerAgent _currentSelected;
        private HashSet<PlayerAgent> _nearbyPlayers = new();
        
        private void Awake()
        {
            InputController.OnInteractPressed += TryPlayerToggle;
            InputController.OnPlayerAgentClicked += HandleSelectionChanged;
        }

        private void Start()
        {
            SetLit(false);
        }

        private void OnDestroy()
        {
            InputController.OnInteractPressed -= TryPlayerToggle;
            InputController.OnPlayerAgentClicked -= HandleSelectionChanged;
        }

        private void Update()
        {
            if (_isLit)
            {
                float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);
                torchLight.intensity = Mathf.Lerp(intensityMin, intensityMax, noise);
                torchLight.pointLightOuterRadius = Mathf.Lerp(radiusMin, radiusMax, noise);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Agent agent = other.GetComponent<Agent>();
            if (!agent) return;

            if (agent is PlayerAgent player)
            {
                _nearbyPlayers.Add(player);
                UpdateFeedback();
            }
            else if (agent is EnemyAgent enemyAgent && _isLit)
                if (UnityEngine.Random.value < enemyAgent.ExtinguishTorchChance)
                    SetLit(false);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            PlayerAgent player = other.GetComponent<PlayerAgent>();
            if (player && _nearbyPlayers.Remove(player))
                UpdateFeedback();
        }

        private void TryPlayerToggle()
        {
            if (_isLit) return;
            if (_currentSelected && _nearbyPlayers.Contains(_currentSelected))
                SetLit(true);
        }

        private void HandleSelectionChanged(PlayerAgent newSelection)
        {
            _currentSelected = newSelection;
            UpdateFeedback();
        }

        private void SetLit(bool litState)
        {
            _isLit = litState;

            if (_isLit) OnLit?.Invoke();

            animator.SetBool(IsLit, _isLit);
            torchLight.enabled = _isLit;
            lit.enabled = _isLit;
            unlit.enabled = !_isLit;

            UpdateFeedback();
        }

        private void UpdateFeedback()
        {
            bool show = !_isLit &&
                        _currentSelected &&
                        _nearbyPlayers.Contains(_currentSelected);

            feedback.gameObject.SetActive(show);
        }
    }
}
