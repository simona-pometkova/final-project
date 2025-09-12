using DungeonGeneration;
using Gameplay.Agents;
using Gameplay.Items;
using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// Orchestrates dungeon generation, rendering
    /// and main game loop.
    /// </summary>
    public class DungeonController : MonoBehaviour
    {
        [Header("Dungeon Size")]
        [SerializeField] private int dungeonWidth = 50;
        [SerializeField] private int dungeonHeight = 50;

        [Header("Node & Room Settings")]
        [SerializeField] private int minNodeSize = 10;
        [SerializeField] private int maxNodeSize = 20;

        [Header("Renderer")]
        [SerializeField] private DungeonRenderer dungeonRenderer;

        [Header("Controllers")] 
        [SerializeField] private AgentsController agentsController;
        [SerializeField] private ItemsController itemsController;

        private DungeonData _dungeon;

        private void Start()
        {
            // Generate dungeon
            DungeonGenerator generator = new DungeonGenerator(dungeonWidth, dungeonHeight, minNodeSize, maxNodeSize);
            generator.GenerateDungeon();

            _dungeon = generator.Dungeon;

            // Render dungeon
            dungeonRenderer.DrawDungeon(_dungeon);
           
            // Create agents
            agentsController.SpawnAgents(_dungeon.Rooms);
            
            // Create items
            itemsController.SpawnItems(_dungeon.Rooms);
        }
    }
}
