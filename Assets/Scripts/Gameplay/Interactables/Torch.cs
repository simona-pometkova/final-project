using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.U2D;

namespace Gameplay.Interactables
{
    // TODO
    public class Torch : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Renderer lit;
        [SerializeField] private Renderer unlit;
        [SerializeField] private Light2D light;

        [Header("Light Flicker Settings")]
        [SerializeField] private float intensityMin;
        [SerializeField] private float intensityMax;
        [SerializeField] private float radiusMin;
        [SerializeField] private float radiusMax;
        [SerializeField] private float flickerSpeed;

        private bool _isLit;

        private void Start()
        {
            light.enabled = false;
            lit.enabled = false;
            animator.SetBool("IsLit", false);
        }

        private void Update()
        {
            if (_isLit)
            {
                float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);
                light.intensity = Mathf.Lerp(intensityMin, intensityMax, noise);
                light.pointLightOuterRadius = Mathf.Lerp(radiusMin, radiusMax, noise);
            }
        }

        public void Toggle()
        {
            _isLit = !_isLit;

            animator.SetBool("IsLit", _isLit);
            light.enabled = _isLit;
            lit.enabled = _isLit;
            unlit.enabled = !_isLit;
        }
    }
}
