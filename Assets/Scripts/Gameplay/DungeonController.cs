using DungeonGeneration;
using Gameplay.Agents;
using Gameplay.Items;
using Gameplay.Levels;
using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// Orchestrates dungeon generation, rendering
    /// and main game loop.
    /// </summary>
    public class DungeonController : MonoBehaviour
    {
        [Header("Renderer")]
        [SerializeField] private DungeonRenderer dungeonRenderer;

        [Header("Controllers")] 
        [SerializeField] private AgentsController agentsController;
        [SerializeField] private ItemsController itemsController;

        private DungeonData _dungeon;

        public void LoadLevel(LevelData level)
        {
            // Generate dungeon
            DungeonGenerator generator = new DungeonGenerator(
                    level.DungeonWidth,
                    level.DungeonHeight,
                    level.MinNodeSize,
                    level.MaxNodeSize
                );

            generator.GenerateDungeon();
            _dungeon = generator.Dungeon;

            // Render dungeon
            dungeonRenderer.DrawDungeon(_dungeon);

            // Move camera to center of dungeon
            float centerX = level.DungeonWidth / 2;
            float centerY = level.DungeonHeight / 2;

            UnityEngine.Camera.main.transform.position = new Vector3(centerX, centerY, UnityEngine.Camera.main.transform.position.z);

            // Create agents
            agentsController.SpawnAgents(_dungeon.Rooms, level);

            // Create items
            itemsController.SpawnItems(_dungeon.Rooms, level);
        }
    }
}
