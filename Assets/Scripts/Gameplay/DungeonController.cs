using DungeonGeneration;
using Gameplay.Agents;
using Gameplay.Items;
using Gameplay.Levels;
using System;
using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// Orchestrates dungeon generation, rendering
    /// and agents and items instantiation
    /// depending on the current level specifications.
    /// </summary>
    public class DungeonController : MonoBehaviour
    {
        // Used to notify the game that the level has been generated
        public static event Action<bool> OnLevelLoaded;

        [Header("Renderer")]
        [SerializeField] private DungeonRenderer dungeonRenderer;

        [Header("Controllers")] 
        [SerializeField] private AgentsController agentsController;
        [SerializeField] private ItemsController itemsController;

        // Cache the current dungeon data
        private DungeonData _dungeon;

        /// <summary>
        /// Loads a level using provided level specifications.
        /// Generates a dungeon, draws it in Unity, and spawns
        /// agents and items.
        /// </summary>
        /// <param name="level">The current level specifications.</param>
        public void LoadLevel(LevelData level)
        {
            // Generate dungeon and cache it
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

            OnLevelLoaded?.Invoke(true);
        }
    }
}
