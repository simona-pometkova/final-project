using Gameplay.Items;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Levels
{
    public class LevelProgressionController : MonoBehaviour
    {
        public LevelData CurrentLevel => levels[_currentLevelIndex];
        
        [SerializeField] private List<LevelData> levels;
        [SerializeField] private DungeonController dungeonController;

        private int _currentLevelIndex;

        private void Start()
        {
            _currentLevelIndex = 0;
            StartLevel();
            ItemsController.OnAllTorchesLit += NextLevel;
        }

        private void OnDestroy()
        {
            ItemsController.OnAllTorchesLit -= NextLevel;
        }

        public void NextLevel()
        {
            _currentLevelIndex++;
            if (_currentLevelIndex < levels.Count) 
                StartLevel();
        }

        public void StartLevel()
        {
            dungeonController.LoadLevel(CurrentLevel);
        }
    }
}
