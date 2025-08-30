using DungeonGeneration.BinarySpacePartitioning;
using UnityEngine;

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
    
        [Header("Node & Room Settings")]
        [SerializeField] private int minNodeSize = 10; 
        [SerializeField] private int maxNodeSize = 20;
    
        [Header("Prefabs")]
        [SerializeField] private GameObject floorTilePrefab;
        [SerializeField] private GameObject wallTilePrefab;

        [Header("Transforms")] 
        [SerializeField] private Transform roomsParent;
        [SerializeField] private Transform corridorsParent;
        [SerializeField] private Transform wallsParent;

        private GameObject[,] _dungeonGameObject;

        /// <summary>
        /// Main entry point — generates a dungeon and visualises it in the Unity Scene.
        /// </summary>
        private void Start()
        {
            // Create a dungeon generator object and generate a dungeon
            DungeonGenerator generator = new DungeonGenerator(dungeonWidth, dungeonHeight, minNodeSize, maxNodeSize);
            generator.GenerateDungeon();
            
            _dungeonGameObject = new GameObject[dungeonWidth, dungeonHeight];
            
            // Draw the dungeon into Unity
            DrawDungeon(generator.Dungeon);
        }

        /// <summary>
        /// Draws the dungeon into the Unity Scene by instantiating
        /// appropriate tile GameObjects (1 - floor, 0 - wall) on each coordinate.
        /// </summary>
        /// <param name="dungeon">The dungeon data to use.</param>
        private void DrawDungeon(DungeonData dungeon)
        {
            // First draw all room tiles
            foreach (Room room in dungeon.Rooms)
            {
                // Local room grid to world dungeon grid
                room.TranslateToGlobalGrid(dungeon.Grid);
                GameObject roomGameObject = new GameObject("Room");
                roomGameObject.transform.SetParent(roomsParent);

                // Create a GameObject for each floor tile of the room
                foreach (Vector2Int tile in room.FloorTiles)
                    CreateGameObject(floorTilePrefab, tile.x, tile.y, roomGameObject.transform);
            }

            // Iterate over dungeon and fill out 
            // corridors and walls on the remaining empty coordinates
            for (int x = 0; x < dungeon.Width; x++)
                for (int y = 0; y < dungeon.Height; y++)
                    if (dungeon.Grid[x, y] == 1 && _dungeonGameObject[x, y] == null)
                        CreateGameObject(floorTilePrefab, x, y, corridorsParent);
                    else if (dungeon.Grid[x, y] == 0)
                        CreateGameObject(wallTilePrefab, x, y, wallsParent);
        }

        /// <summary>
        /// Instantiates a GameObject and updates
        /// the main dungeon GameObject.
        /// </summary>
        /// <param name="prefab">The prefab to use for instantiation.</param>
        /// <param name="positionX">The x-position to instantiate the GameObject on.</param>
        /// <param name="positionY">The y-position to instantiate the GameObject on.</param>
        /// <param name="parent">The parent of the GameObject.</param>
        private void CreateGameObject(GameObject prefab, int positionX, int positionY, Transform parent)
        {
            Vector3 position = new Vector3(positionX, positionY, 0);
            GameObject go = Instantiate(prefab, position, Quaternion.identity, parent);
            _dungeonGameObject[positionX, positionY] = go;
        }
    }
}