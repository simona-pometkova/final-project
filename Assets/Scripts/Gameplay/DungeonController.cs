using DungeonGeneration;
using DungeonGeneration.BinarySpacePartitioning;
using System.Collections.Generic;
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

        [Header("Agents")]
        [SerializeField] private Transform parent;
        [SerializeField] private GameObject playerAgent;
        [SerializeField] private GameObject enemyAgent;

        [Header("Renderer")]
        [SerializeField] private DungeonRenderer renderer;

        private DungeonData _dungeon;

        private void Start()
        {
            // Generate dungeon
            DungeonGenerator generator = new DungeonGenerator(dungeonWidth, dungeonHeight, minNodeSize, maxNodeSize);
            generator.GenerateDungeon();

            _dungeon = generator.Dungeon;

            // Render dungeon
            renderer.DrawDungeon(_dungeon);

            SpawnAgents(_dungeon.Rooms);
        }

        private void SpawnAgents(List<Room> rooms)
        {
            Vector2Int spawnTile = rooms[0].FloorTiles[Random.Range(0, rooms[0].FloorTiles.Count)];
            Vector3 spawnPosition = new Vector3(spawnTile.x + 0.5f, spawnTile.y + 0.5f, 0);
            Instantiate(playerAgent, spawnPosition, Quaternion.identity, parent);
        }
    }
}
