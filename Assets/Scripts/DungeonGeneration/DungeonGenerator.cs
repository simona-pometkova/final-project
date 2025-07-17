using DungeonGeneration.BinarySpacePartitioning;
using UnityEngine;

namespace DungeonGeneration
{
    /// <summary>
    /// Handles procedural generation of a dungeon using the Binary Space Partitioning (BSP) algorithm.
    /// Performs the following steps:
    /// 1. Recursively splits the map space into smaller subregions.
    /// 2. Creates a room in each leaf node.
    /// 3. Connects the rooms using corridors.
    /// 4. Instantiates floor and corridor tiles in the scene.
    /// </summary>
    public class DungeonGenerator : MonoBehaviour
    {
        [Header("Dungeon Size")]
        [SerializeField] private int dungeonRows = 50;
        [SerializeField] private int dungeonColumns = 50;
    
        [Header("Room Settings")]
        [SerializeField] private int minRoomSize = 10; 
        [SerializeField] private int maxRoomSize = 20;
    
        [Header("Prefabs")]
        [SerializeField] private GameObject floorTilePrefab;
        [SerializeField] private GameObject corridorTilePrefab;

        private GameObject[,] _dungeon;

        // 25% chance to split. Adds variety
        private const float SplitChanceThreshold = 0.75f;

        /// <summary>
        /// Main entry point — starts BSP dungeon generation.
        /// </summary>
        private void Start()
        {
            // Create the main room (root node in the BSP tree) that takes up the whole size of the dungeon.
            Node rootNode = new Node(new Rect(0, 0, dungeonRows, dungeonColumns));
        
            // Recursively partition the dungeon space.
            Partition(rootNode);
        
            // Create rooms inside each node.
            rootNode.CreateRoom();

            // Initialize the dungeon GameObject.
            _dungeon = new GameObject[dungeonRows, dungeonColumns];
        
            // Draw rooms and corridors.
            DrawRooms(rootNode);
            DrawCorridors(rootNode);
        }

        /// <summary>
        /// Starting from the root node, recursively partitions the space into
        /// subspaces (sub-dungeons) until a leaf node is reached. 
        /// </summary>
        /// <param name="node">The subspace (BSP node) that will be partitioned.</param>
        private void Partition(Node node)
        {
            if (!node.IsLeaf()) return;
        
            // Check if node should be split (either too big or randomly decided)
            if (node.Rect.width > maxRoomSize      
                || node.Rect.height > maxRoomSize
                || Random.Range(0.0f, 1.0f) > SplitChanceThreshold) 
            {
                // If the sub-dungeon was successfully split, proceed to recursively partition its children
                if (node.Split(minRoomSize))
                {
                    Partition(node.LeftChild);
                    Partition(node.RightChild);
                }
            }
        }

        /// <summary>
        /// Recursively instantiates floor tile GameObjects for each room.
        /// </summary>
        /// <param name="node">The node to start drawing rooms from.</param>
        private void DrawRooms(Node node)
        {
            if (node == null) return;

            if (node.IsLeaf())
            {
                for (int i = (int)node.Room.x; i < node.Room.xMax; i++)
                {
                    for (int j = (int)node.Room.y; j < node.Room.yMax; j++)
                    {
                        // Instantiate GameObject
                        GameObject instance =
                            Instantiate(floorTilePrefab, new Vector3(i, j, 0f), Quaternion.identity);
                        instance.transform.SetParent(transform);
                    
                        _dungeon[i, j] = instance;
                    }
                }
            }
            else
            {
                DrawRooms(node.LeftChild);
                DrawRooms(node.RightChild);
            }
        }

        /// <summary>
        /// Recursively instantiates corridor tile GameObjects for each corridor in the BSP tree.
        /// Only instantiates corridor tiles if no tile already exists at the position.
        /// </summary>
        /// <param name="node">The node to start drawing corridors from.</param>
        private void DrawCorridors(Node node)
        {
            if (node == null) return;

            DrawCorridors(node.LeftChild);
            DrawCorridors(node.RightChild);

            foreach (Rect corridor in node.Corridors)
            {
                for (int i = (int)corridor.x; i < corridor.xMax; i++)
                {
                    for (int j = (int)corridor.y; j < corridor.yMax; j++)
                    {
                        if (_dungeon[i, j] != null) continue;
                    
                        // Instantiate GameObject
                        GameObject instance =
                            Instantiate(corridorTilePrefab, new Vector3(i, j, 0f), Quaternion.identity);
                        instance.transform.SetParent(transform);
                        
                        _dungeon[i, j] = instance;
                    }
                }
            }
        }
    }
}