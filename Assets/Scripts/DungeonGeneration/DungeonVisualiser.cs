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
            
            DrawDungeon(generator.Dungeon);
        }

        //TODO document
        private void DrawDungeon(DungeonData dungeon)
        {
            foreach (Room room in dungeon.Rooms)
            {
                room.TranslateToGlobalGrid(dungeon.Grid);
                GameObject roomGameObject = new GameObject("Room");
                roomGameObject.transform.SetParent(roomsParent);

                foreach (Vector2Int tile in room.FloorTiles)
                {
                    Vector3 position = new Vector3(tile.x, tile.y, 0);
                    GameObject floor = Instantiate(floorTilePrefab, position, Quaternion.identity, roomGameObject.transform);
                    _dungeonGameObject[tile.x, tile.y] = floor;
                }
            }

            for (int x = 0; x < dungeon.Width; x++)
            {
                for (int y = 0; y < dungeon.Height; y++)
                {
                    if (dungeon.Grid[x, y] == 1 && _dungeonGameObject[x, y] == null)
                    {
                        Vector3 position = new Vector3(x, y, 0);
                        GameObject floor = Instantiate(floorTilePrefab, position, Quaternion.identity, corridorsParent);
                        _dungeonGameObject[x, y] = floor;
                    }
                    else if (dungeon.Grid[x, y] == 0)
                    {
                        Vector3 position = new Vector3(x, y, 0);
                        GameObject wall = Instantiate(wallTilePrefab, position, Quaternion.identity, wallsParent);
                    }
                }
            }
        }
    }
}