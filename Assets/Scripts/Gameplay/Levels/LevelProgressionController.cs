using Gameplay.Agents;
using Gameplay.Items;
using Gameplay.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Levels
{
    // TODO documentation
    public class LevelProgressionController : MonoBehaviour
    {
        public static event Action<bool> OnLevelCompleted;
        public LevelData CurrentLevel => levels[_currentLevelIndex];
        
        [SerializeField] private List<LevelData> levels;
        [SerializeField] private FadeScreen fadeScreen;
        [SerializeField] private DungeonController dungeonController;

        private int _currentLevelIndex;

        private void Start()
        {
            ItemsController.OnAllTorchesLit += NextLevel;
            AgentsController.OnAgentConverted += GameOverCheck;
            InputController.OnQuitPressed += Quit;
        }

        private void OnDestroy()
        {
            ItemsController.OnAllTorchesLit -= NextLevel;
            AgentsController.OnAgentConverted -= GameOverCheck;
            InputController.OnQuitPressed -= Quit;
        }

        public void OnBeginButton()
        {
            fadeScreen.FadeToTransparent(this.StartLevel);
        }

        public void NextLevel()
        {
            OnLevelCompleted?.Invoke(false);
            _currentLevelIndex++;
            if (_currentLevelIndex < levels.Count)
            {
                fadeScreen.FadeToBlack(null, () =>
                {
                    fadeScreen.FadeToTransparent();
                    StartLevel();
                });
            }
            else
                Win();
        }

        public void StartLevel() => dungeonController.LoadLevel(CurrentLevel);

        private void GameOverCheck(int players, int enemies)
        {
            if (players == 0)
                GameOver();
        }

        private void GameOver()
        {
            ResetGame(fadeScreen.GameOver);
        }

        private void Win()
        {
            ResetGame(fadeScreen.Win);
        }

        public void Quit()
        {
            ResetGame(fadeScreen.MainMenu);
        }

        private void ResetGame(GameObject screen)
        {
            _currentLevelIndex = 0;
            OnLevelCompleted?.Invoke(false);
            fadeScreen.FadeToBlack(screen);
        }
    }
}
