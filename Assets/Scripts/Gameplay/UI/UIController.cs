using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Gameplay.Items;
using NUnit.Framework;
using System.Collections.Generic;
using Gameplay.Agents;
using Gameplay.Levels;

namespace Gameplay.UI
{
    /// <summary>
    /// Manages all gameplay UI elements including level indicators,
    /// counters for torches, players, and enemies,
    /// and fade screen transitions.
    /// </summary>
    public class UIController : MonoBehaviour
    {
        [SerializeField] private FadeScreen fadeScreen;
        [SerializeField] private Image level;
        [SerializeField] private TextMeshProUGUI torchesCounter;
        [SerializeField] private TextMeshProUGUI playersCounter;
        [SerializeField] private TextMeshProUGUI enemiesCounter;

        // Active game entities used for UI updates
        private readonly List<Torch> _torches = new();
        private readonly List<PlayerAgent> _players = new();
        private readonly List<EnemyAgent> _enemies = new();

        /// <summary>
        /// Subscribes to gameplay events
        /// and initializes the Main Menu screen.
        /// </summary>
        private void Start()
        {
            DungeonController.OnLevelLoaded += ShowLevel;
            LevelProgressionController.OnLevelCompleted += ShowLevel;
            ItemsController.OnTorchesSpawned += GetTorches;
            AgentsController.OnAgentsSpawned += GetAgents;
            AgentsController.OnAgentConverted += SetAgentCounters;
            Torch.OnStateChanged += SetTorchesCounter;

            ShowMainMenu();
        }

        /// <summary>
        /// Shows the main menu by fading the screen to black
        /// and activating the menu UI.
        /// </summary>
        public void ShowMainMenu() => fadeScreen.FadeToBlack(fadeScreen.MainMenu);

        private void OnDestroy()
        {
            DungeonController.OnLevelLoaded -= ShowLevel;
            LevelProgressionController.OnLevelCompleted -= ShowLevel;
            ItemsController.OnTorchesSpawned -= GetTorches;
            AgentsController.OnAgentsSpawned -= GetAgents;
            AgentsController.OnAgentConverted -= SetAgentCounters;
            Torch.OnStateChanged -= SetTorchesCounter;
        }

        /// <summary>
        /// Shows or hides the level UI indicator
        /// based on the current game state.
        /// </summary>
        /// <param name="show">True to show the level UI, false to hide it.</param>
        private void ShowLevel(bool show) => level.gameObject.SetActive(show);

        /// <summary>
        /// Caches all torches present in the current level
        /// and updates the torch counter.
        /// </summary>
        /// <param name="torches">A list of all torches spawned in the level.</param>
        private void GetTorches(List<Torch> torches)
        {
            _torches.Clear();
            _torches.AddRange(torches);
            SetTorchesCounter();
        }

        /// <summary>
        /// Caches all player and enemy agents present in the current level
        /// and updates the agent counters.
        /// </summary>
        /// <param name="playerAgents">List of player agents currently active.</param>
        /// <param name="enemyAgents">List of enemy agents currently active.</param>
        private void GetAgents(List<PlayerAgent> playerAgents, List<EnemyAgent> enemyAgents)
        {
            _players.Clear();
            _enemies.Clear();
            _players.AddRange(playerAgents);
            _enemies.AddRange(enemyAgents);

            SetAgentCounters(_players.Count, _enemies.Count);
        }

        /// <summary>
        /// Updates the UI counter showing the number of lit torches.
        /// </summary>
        /// <param name="lit">Unused parameter. Included to match the event signature.</param>
        private void SetTorchesCounter(bool lit = true)
        {
            int litTorches = 0;

            // Count all torches currently lit
            foreach (var torch in _torches)
                if (torch.IsTorchLit) 
                    litTorches++;

            torchesCounter.text = $"<color=#ff7215>Torches: {litTorches}/{_torches.Count}</color>";
        }

        /// <summary>
        /// Updates the UI counters showing
        /// the current number of players and enemies.
        /// </summary>
        /// <param name="playersCount">Current number of active player agents.</param>
        /// <param name="enemiesCount">Current number of active enemy agents.</param>
        private void SetAgentCounters(int playersCount, int enemiesCount)
        {
            playersCounter.text = $"<color=#1667ff>Players: {playersCount}</color>";
            enemiesCounter.text = $"<color=#fe0000>Enemies: {enemiesCount}</color>";
        }
    }
}
