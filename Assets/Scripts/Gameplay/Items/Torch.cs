using Gameplay.Agents;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Gameplay.Items
{
    // TODO
    public class Torch : MonoBehaviour
    {
        private static readonly int IsLit = Animator.StringToHash("IsLit");
        
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
        private PlayerAgent _nearbyPlayer;
        
        private void Awake()
        {
            InputController.OnInteractPressed += Toggle;
        }

        private void Start()
        {
            torchLight.enabled = false;
            lit.enabled = false;
            animator.SetBool(IsLit, false);
        }

        private void OnDestroy()
        {
            InputController.OnInteractPressed -= Toggle;
        }

        // TODO this is messed up - nearby player check is incorrect
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isLit) return;
            
            _nearbyPlayer = other.GetComponent<PlayerAgent>();
            
            if (_nearbyPlayer != null && _nearbyPlayer.IsCurrentlySelected)
                ShowFeedback(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            ShowFeedback(false);
            
            if (_nearbyPlayer != null && _nearbyPlayer.IsCurrentlySelected)
                _nearbyPlayer = null;
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

        private void Toggle(bool isLit)
        {
            if (_nearbyPlayer != null && _nearbyPlayer.IsCurrentlySelected)
            {
                _isLit = isLit;

                animator.SetBool(IsLit, isLit);
                torchLight.enabled = isLit;
                lit.enabled = isLit;
                unlit.enabled = !isLit;
            }
            
            if (_isLit) ShowFeedback(false);
        }
        
        private void ShowFeedback(bool show) => feedback.gameObject.SetActive(show);
    }
}
