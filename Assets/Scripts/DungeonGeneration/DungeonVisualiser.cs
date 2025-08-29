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
            
            //TODO document
            foreach (var room in generator.Dungeon.Rooms)
                room.TranslateToGlobalGrid(generator.Dungeon.Grid);

            for (int x = 0; x < dungeonWidth; x++)
            {
                for (int y = 0; y < dungeonHeight; y++)
                {
                    if (generator.Dungeon.Grid[x, y] == 1)
                        Instantiate(roomFloorTilePrefab, new Vector3(x, y, 0), Quaternion.identity, roomsParent);
                    else if (generator.Dungeon.Grid[x, y] == 2)
                        Instantiate(corridorFloorTilePrefab, new Vector3(x, y, 0), Quaternion.identity, corridorsParent);
                }
            }
        }

        // private void Draw(Room room, GameObject floorPrefab, GameObject wallPrefab, string gameObjectName, Transform parent)
        // {
        //     int[,] grid = room.Grid;
        //     int width = grid.GetLength(0);
        //     int height = grid.GetLength(1);
        //
        //     GameObject holder = new GameObject(gameObjectName);
        //
        //     for (int x = 0; x < width; x++)
        //     {
        //         for (int y = 0; y < height; y++)
        //         {
        //             int value = grid[x, y];
        //             if (value == 1)
        //             {
        //                 // Position relative to room
        //                 Vector3 position = new Vector3(room.Bounds.x + x, room.Bounds.y + y, 0);
        //                 GameObject tile = Instantiate(floorPrefab, position, Quaternion.identity, holder.transform);
        //
        //                 _dungeonGameObject[(int)(room.Bounds.x + x), (int)(room.Bounds.y + y)] = tile;
        //             }
        //         }
        //     }
        //
        //     if (holder.transform.childCount > 0)
        //         holder.transform.SetParent(parent);
        //     else Destroy(holder);
        // }
    }
}