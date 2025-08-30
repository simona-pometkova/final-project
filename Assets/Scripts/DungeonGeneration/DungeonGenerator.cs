using System.Collections.Generic;
using DungeonGeneration.BinarySpacePartitioning;
using UnityEngine;
using Utils;

namespace DungeonGeneration
{
    /// <summary>
    /// A class that holds data about the dungeon,
    /// such as rooms and corridors. Used to separate
    /// procedural generation logic from Unity-specific code
    /// and allow for testability.
    /// </summary>
    public class DungeonData
    {
        public readonly List<Room> Rooms = new();
        public int Width { get; }
        public int Height { get; }
        public int MinNodeSize { get; }
        public int MaxNodeSize { get; }
        public int[,] Grid { get; }
        
        public DungeonData(int width, int height, int minNodeSize, int maxNodeSize)
        {
            this.Width = width;
            this.Height = height;
            this.MinNodeSize = minNodeSize;
            this.MaxNodeSize = maxNodeSize;
            this.Grid = new int[width, height];
        }        
    }
    
    /// <summary>
    /// Handles procedural generation of a dungeon using the Binary Space Partitioning (BSP) algorithm.
    /// Performs the following steps:
    /// 1. Recursively splits the map space into smaller subregions.
    /// 2. Creates a room in each leaf node.
    /// 3. Connects the rooms using corridors.
    /// 4. Saves the room and corridors data.
    /// </summary>
    public class DungeonGenerator
    {
        public DungeonData Dungeon => _dungeon;
        
        // 25% chance to split node
        // TODO export as Serializable field?
        private const float SplitChanceThreshold = 0.75f;
        
        private readonly DungeonData _dungeon;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="width">The width (number of columns) of the dungeon.</param>
        /// <param name="height">The height (number of rows) of the dungeon.</param>
        /// <param name="minNodeSize">Minimum allowable node size.</param>
        /// <param name="maxNodeSize">Maximum allowable node size.</param>
        public DungeonGenerator(int width, int height, int minNodeSize, int maxNodeSize)
        {
            this._dungeon = new DungeonData(width, height, minNodeSize, maxNodeSize);
        }

        /// <summary>
        /// Generates a dungeon layout using the BSP algorithm.
        /// </summary>
        public void GenerateDungeon()
        {
            // Create the main space (root node in the BSP tree) that takes up the whole size of the dungeon
            BSPNode rootNode = new BSPNode(new Rect(0, 0, _dungeon.Width, _dungeon.Height));
            
            // Recursively partition the dungeon space
            Partition(rootNode);
            
            // Create rooms and corridors
            rootNode.CreateRooms();
            rootNode.CreateCorridors(_dungeon.Grid);
            
            // Save rooms data
            CollectRooms(rootNode, _dungeon.Rooms);
        }

        /// <summary>
        /// Starting from the root node, recursively subdivides the tree
        /// into subspaces (sub-dungeons) until a leaf node is reached. 
        /// </summary>
        /// <param name="node">The subspace (BSP node) that will be partitioned.</param>
        private void Partition(BSPNode node)
        {
            // Check if node should be split (either too big or randomly decided)
            if (node.NodeBounds.width > _dungeon.MaxNodeSize      
                || node.NodeBounds.height > _dungeon.MaxNodeSize
                || Maths.GetRandomFloat(0.0f, 1.0f) > SplitChanceThreshold) 
            {
                // If the sub-dungeon was successfully split, proceed to recursively partition its children
                if (node.Split(_dungeon.MinNodeSize))
                {
                    Partition(node.LeftChild);
                    Partition(node.RightChild);
                }
            }
        }

        /// <summary>
        /// Recursively collects the rooms from all leaf nodes in a BSP tree.
        /// </summary>
        /// <param name="node">The current node to process.</param>
        /// <param name="rooms">The list to which all discovered rooms will be added.</param>
        private void CollectRooms(BSPNode node, List<Room> rooms)
        {
            if (node == null) return;

            if (node.IsLeaf())
            {
                Room room = node.GetRoom();

                if (room != null)
                    rooms.Add(room);
            }
            else
            {
                CollectRooms(node.LeftChild, rooms);
                CollectRooms(node.RightChild, rooms);
            }
        }
    }
}