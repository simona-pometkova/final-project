using Gameplay.Agents;
using UnityEngine;

namespace Gameplay.Levels
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]
    public class LevelData : ScriptableObject
    {
        public int DungeonWidth => dungeonWidth;
        public int DungeonHeight => dungeonHeight;
        public int MinNodeSize => minNodeSize;
        public int MaxNodeSize => maxNodeSize;
        public int TorchesPerRoom => torchesPerRoom;
        public int MinimumTorchDistance => minimumTorchDistance;
        public int PlayerAgentsCount => playerAgentsCount;
        public int EnemyAgentsCount => enemyAgentsCount;
        public AgentData AgentData => agentData;

        [Header("Dungeon Settings")]
        [SerializeField] private int dungeonWidth;
        [SerializeField] private int dungeonHeight;
        [SerializeField] private int minNodeSize;
        [SerializeField] private int maxNodeSize;

        [Header("Item Settings")]
        [SerializeField] private int torchesPerRoom;

        [SerializeField] private int minimumTorchDistance;

        [Header("Agent Settings")]
        [SerializeField] private int playerAgentsCount;
        [SerializeField] private int enemyAgentsCount;
        [SerializeField] private AgentData agentData;
    }
}
