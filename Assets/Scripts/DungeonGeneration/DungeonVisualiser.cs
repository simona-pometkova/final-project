using DungeonGeneration.BinarySpacePartitioning;
using UnityEngine;
using UnityEngine.Serialization;

namespace DungeonGeneration
{
    /// <summary>
    /// A Unity class that holds configurable values for the dungeon
    /// generation and visualises everything in the Scene via GameObjects.
    /// </summary>
    public class DungeonVisualiser : MonoBehaviour
    {
        [Header("Dungeon Size")]
        [SerializeField] private int dungeonWidth = 50;
        [SerializeField] private int dungeonHeight = 50;
    
        [Header("Room Settings")]
        [SerializeField] private int minRoomSize = 10; 
        [SerializeField] private int maxRoomSize = 20;
    
        [Header("Prefabs")]
        [SerializeField] private GameObject roomTilePrefab;
        [SerializeField] private GameObject corridorTilePrefab;

        private GameObject[,] _dungeon;

        /// <summary>
        /// Main entry point — generates a dungeon and visualises it in the Unity Scene.
        /// </summary>
        private void Start()
        {
            // Create a dungeon generator object and generate a dungeon.
            DungeonGenerator generator = new DungeonGenerator(dungeonWidth, dungeonHeight, minRoomSize, maxRoomSize);
            generator.GenerateDungeon();
            
            _dungeon = new GameObject[dungeonWidth, dungeonHeight];

            // Get the dungeon's data.
            DungeonData data = generator.GetData();
            
            // Create GameObjects for each room in the dungeon.
            foreach (var room in data.Rooms)
                DrawTiles(room.Bounds, roomTilePrefab);

            // Crete GameObjects for each corridor in the dungeon.
            foreach (var corridor in data.Corridors)
                DrawTiles(corridor, corridorTilePrefab);
        }

        /// <summary>
        /// Fills the specified area of the dungeon grid with tile prefabs,
        /// unless a tile already exists at a given coordinate.
        /// </summary>
        /// <param name="rect">The rectangular area to fill with tiles.</param>
        /// <param name="prefab">The prefab to instantiate for each empty grid coordinate.</param>
        private void DrawTiles(Rect rect, GameObject prefab)
        {
            // Traverse the area.
            for (int i = (int)rect.xMin; i < rect.xMax; i++)
            {
                for (int j = (int)rect.yMin; j < rect.yMax; j++)
                {
                    // Don't do anything if the position already contains a tile.
                    if (_dungeon[i, j] != null) continue;
                    
                    // Instantiate tile GameObject and assign it to dungeon coordinate.
                    GameObject tile = Instantiate(prefab, new Vector3(i, j, 0), Quaternion.identity, transform);
                    _dungeon[i, j] = tile;
                }
            }
        }
    }
}