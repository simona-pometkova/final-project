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
        [SerializeField] private GameObject roomFloorTilePrefab;
        [SerializeField] private GameObject roomWallTilePrefab;
        [SerializeField] private GameObject corridorFloorTilePrefab;

        [Header("Transforms")] 
        [SerializeField] private Transform roomsParent;
        [SerializeField] private Transform corridorsParent;

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

            // Create GameObjects for each room in the dungeon
            foreach (var room in generator.Dungeon.Rooms)
                Draw(room, roomFloorTilePrefab, roomWallTilePrefab, "Room", roomsParent);

            // Create GameObjects for each corridor in the dungeon
            foreach (var corridor in generator.Dungeon.Corridors)
                foreach (var segment in corridor.Segments)
                    DrawTiles(segment, corridorFloorTilePrefab, "Corridor", corridorsParent);
        }

        /// <summary>
        /// Fills the specified area of the dungeon grid with tile prefabs,
        /// unless a tile already exists at a given coordinate.
        /// The tiles are parented by a common game object that is
        /// then added to the Unity Scene.
        /// </summary>
        /// <param name="rect">The rectangular area to fill with tiles.</param>
        /// <param name="tilePrefab">The prefab to instantiate for each empty grid coordinate.</param>
        /// <param name="gameObjectName">The name to give to the game object that holds all tiles.</param>
        /// <param name="parent">The parent to attach the holder game object to.</param>
        private void DrawTiles(Rect rect, GameObject tilePrefab, string gameObjectName, Transform parent)
        {
            // Create a parent that will hold all tiles that belong
            // to the same room/corridor
            GameObject holder = new GameObject(gameObjectName);  
            
            // Traverse the area
            for (int i = (int)rect.xMin; i < rect.xMax; i++)
            {
                for (int j = (int)rect.yMin; j < rect.yMax; j++)
                {
                    // Don't do anything if the position already contains a tile
                    if (_dungeonGameObject[i, j] != null) continue;
                    
                    // Instantiate tile GameObject and assign it to dungeon coordinate
                    GameObject tile = Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity, holder.transform);
                    _dungeonGameObject[i, j] = tile;
                }
            }

            if (holder.transform.childCount > 0)
                holder.transform.SetParent(parent);
            else Destroy(holder);
        }

        private void Draw(Room room, GameObject floorPrefab, GameObject wallPrefab, string gameObjectName, Transform parent)
        {
            int[,] grid = room.Grid;
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);

            GameObject holder = new GameObject(gameObjectName);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int value = grid[x, y];
                    if (value == 1)
                    {
                        // Position relative to room
                        Vector3 position = new Vector3(room.Bounds.x + x, room.Bounds.y + y, 0);
                        GameObject tile = Instantiate(floorPrefab, position, Quaternion.identity, holder.transform);

                        _dungeonGameObject[(int)(room.Bounds.x + x), (int)(room.Bounds.y + y)] = tile;
                    }
                }
            }

            if (holder.transform.childCount > 0)
                holder.transform.SetParent(parent);
            else Destroy(holder);
        }
    }
}