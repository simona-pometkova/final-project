using Gameplay.Levels;
using System;
using System.Collections;
using UnityEngine;

namespace Gameplay.UI
{
    /// <summary>
    /// Controls fade-in and fade-out screen transitions
    /// between game states (e.g., main menu, gameplay, game over, victory).
    /// Can optionally activate/deactivate specific UI screens during the fade process.
    /// </summary>
    public class FadeScreen : MonoBehaviour
    {
        // Used by the main UI & level progression controllers
        public GameObject MainMenu => mainMenu;
        public GameObject GameOver => gameOver;
        public GameObject Win => win;

        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private GameObject gameOver;
        [SerializeField] private GameObject win;
        [SerializeField] private LevelProgressionController levelController;
        [SerializeField] private float fadeDuration = 1.5f;
        [SerializeField] private float delay = 3f;

        // Currently active UI screen to fade in/out
        private GameObject _activeScreen;

        /// <summary>
        /// Starts a fade-out transition from black to transparent,
        /// revealing the underlying gameplay or UI elements.
        /// </summary>
        /// <param name="onComplete">Optional callback invoked when the fade transition finishes.</param>
        public void FadeToTransparent(Action onComplete = null) => StartCoroutine(Fade(1f, 0f, onComplete));

        /// <summary>
        /// Starts a fade-in transition from transparent to black,
        /// optionally activating a specific screen during the fade.
        /// </summary>
        /// <param name="screen">Optional UI screen to activate while fading to black.</param>
        /// <param name="onComplete">Optional callback invoked when the fade transition finishes.</param>
        public void FadeToBlack(GameObject screen = null, Action onComplete = null)
        {
            _activeScreen = screen;
            
            if (_activeScreen)
                _activeScreen.SetActive(true);

            StartCoroutine(Fade(0f, 1f, onComplete));
        }

        /// <summary>
        /// Handles the actual fade animation.
        /// </summary>
        /// <param name="from">Starting alpha value (0 = transparent, 1 = black).</param>
        /// <param name="to">Target alpha value (0 = transparent, 1 = black).</param>
        /// <param name="onComplete">Optional callback executed after the fade completes.</param>
        private IEnumerator Fade(float from, float to, Action onComplete = null)
        {
            float timer = 0f;
            canvasGroup.alpha = from;

            // Gradually interpolate the alpha over time until the fade is complete
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(from, to, timer / fadeDuration);
                yield return null;
            }

            // Ensure final alpha is set precisely to the target value
            canvasGroup.alpha = to;

            // If we faded out to transparency and a screen is active, disable it after a delay
            if (canvasGroup.alpha == 0 && _activeScreen)
            {
                _activeScreen.SetActive(false);
                yield return new WaitForSeconds(delay);
            }
            
            // Invoke completion fallback if provided
            onComplete?.Invoke();
        }
    }
}
