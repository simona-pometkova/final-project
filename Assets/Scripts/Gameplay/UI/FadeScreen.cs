using Gameplay.Items;
using Gameplay.Levels;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI
{
    public class FadeScreen : MonoBehaviour
    {
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

        private GameObject _activeScreen;

        public void FadeToTransparent(Action onComplete = null)
        {
            StartCoroutine(Fade(1f, 0f, onComplete));
        }

        public void FadeToBlack(GameObject screen = null, Action onComplete = null)
        {
            _activeScreen = screen;
            if (_activeScreen)
                _activeScreen.SetActive(true);

            StartCoroutine(Fade(0f, 1f, onComplete));
        }

        private IEnumerator Fade(float from, float to, Action onComplete = null)
        {
            float timer = 0f;
            canvasGroup.alpha = from;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(from, to, timer / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = to;

            if (canvasGroup.alpha == 0 && _activeScreen)
            {
                _activeScreen.SetActive(false);
                yield return new WaitForSeconds(delay);
            }
            
            onComplete?.Invoke();
        }
    }
}
