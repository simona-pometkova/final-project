using Gameplay.Agents;
using Gameplay.Items;
using Gameplay.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Levels
{
    /// <summary>
    /// Controls the overall level flow, including starting levels,
    /// transitioning to the next level, handling game-over conditions,
    /// and resetting the game after a win or quit.
    /// </summary>
    public class LevelProgressionController : MonoBehaviour
    {
        public static event Action<bool> OnLevelCompleted;
        
        [SerializeField] private List<LevelData> levels;
        [SerializeField] private FadeScreen fadeScreen;
        [SerializeField] private DungeonController dungeonController;

        // Currently active level
        private int _currentLevelIndex;
        private LevelData _currentLevel => levels[_currentLevelIndex];

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

        /// <summary>
        /// Called by the UI Begin button to start the game.
        /// Fades the screen to transparent and begins the first level.
        /// </summary>
        public void OnBeginButton() => fadeScreen.FadeToTransparent(StartLevel);

        
        /// <summary>
        /// Advances the game to the next level if available,
        /// or triggers the win sequence if all levels have been completed.
        /// </summary>
        private void NextLevel()
        {
            // Notify subscribers that the current level has ended
            OnLevelCompleted?.Invoke(false);
            _currentLevelIndex++;
            
            
            if (_currentLevelIndex < levels.Count)
            {
                // Fade out and then fade in for a smooth transition to the next level
                fadeScreen.FadeToBlack(null, () =>
                {
                    fadeScreen.FadeToTransparent();
                    StartLevel();
                });
            }
            else
                // All levels completed
                Win();
        }

        /// <summary>
        /// Starts the currently selected level by loading the dungeon layout.
        /// </summary>
        private void StartLevel() => dungeonController.LoadLevel(_currentLevel);

        /// <summary>
        /// Game is over if the player loses all of their agents.
        /// Called whenever an agent is converted.
        /// </summary>
        private void GameOverCheck(int players, int enemies)
        {
            if (players == 0)
                GameOver();
        }

        /// <summary>
        /// Handles game over logic when all player agents are lost.
        /// </summary>
        private void GameOver()
        {
            ResetGame(fadeScreen.GameOver);
        }

        /// <summary>
        /// Handles win logic when all levels have been completed.
        /// </summary>
        private void Win()
        {
            ResetGame(fadeScreen.Win);
        }

        /// <summary>
        /// Handles quitting the level from user input.
        /// </summary>
        public void Quit()
        {
            ResetGame(fadeScreen.MainMenu);
        }

        /// <summary>
        /// Resets the game state back to the start
        /// and displays the specified end screen.
        /// </summary>
        /// <param name="screen">The UI screen to show after fading out (e.g., Win, GameOver, MainMenu).</param>
        private void ResetGame(GameObject screen)
        {
            // Reset to first level
            _currentLevelIndex = 0;
            
            OnLevelCompleted?.Invoke(false);
            fadeScreen.FadeToBlack(screen);
        }
    }
}
