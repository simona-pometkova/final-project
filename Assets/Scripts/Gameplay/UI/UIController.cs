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
    // TODO documentation
    public class UIController : MonoBehaviour
    {
        [SerializeField] private FadeScreen fadeScreen;
        [SerializeField] private Image level;
        [SerializeField] private TextMeshProUGUI torchesCounter;
        [SerializeField] private TextMeshProUGUI playersCounter;
        [SerializeField] private TextMeshProUGUI enemiesCounter;

        private List<Torch> _torches = new();
        private List<PlayerAgent> _players = new();
        private List<EnemyAgent> _enemies = new();

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

        private void ShowLevel(bool show)
        {
            level.gameObject.SetActive(show);
        }

        private void GetTorches(List<Torch> torches)
        {
            _torches.Clear();
            _torches.AddRange(torches);
            SetTorchesCounter();
        }

        private void GetAgents(List<PlayerAgent> playerAgents, List<EnemyAgent> enemyAgents)
        {
            _players.Clear();
            _enemies.Clear();
            _players.AddRange(playerAgents);
            _enemies.AddRange(enemyAgents);

            SetAgentCounters(_players.Count, _enemies.Count);
        }

        private void SetTorchesCounter()
        {
            int litTorches = 0;

            foreach (var torch in _torches)
                if (torch.IsTorchLit) 
                    litTorches++;

            torchesCounter.text = $"<color=#ff7215>Torches: {litTorches}/{_torches.Count}</color>";
        }

        private void SetAgentCounters(int playersCount, int enemiesCount)
        {
            playersCounter.text = $"<color=#1667ff>Players: {playersCount}</color>";
            enemiesCounter.text = $"<color=#fe0000>Enemies: {enemiesCount}</color>";
        }
    }
}
