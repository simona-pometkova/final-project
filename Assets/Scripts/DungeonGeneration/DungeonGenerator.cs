using System.Collections.Generic;
using DungeonGeneration.BinarySpacePartitioning;
using UnityEngine;

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
        public readonly List<Rect> Rooms = new();
        public readonly List<Rect> Corridors = new();
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
        // 25% chance to split. Adds variety
        private const float SplitChanceThreshold = 0.75f;
        
        // TODO: instead of passing those configurables as params, create a constructor for DungeonGenerator
        // TODO: instead of returning data in this method, save it as a field and write a getter
        // TODO: Make DungeonGenerator a Singleton?
        
        /// <summary>
        /// Generates a dungeon layout given the specified grid size and room size constraints.
        /// </summary>
        /// <param name="rows">Number of rows in the dungeon (height).</param>
        /// <param name="columns">Number of columns in the dungeon (width).</param>
        /// <param name="minRoomSize">Minimum allowable room size.</param>
        /// <param name="maxRoomSize">Maximum allowable room size.</param>
        public DungeonData GenerateDungeon(int rows, int columns, int minRoomSize, int maxRoomSize)
        {
            // Create the main space (root node in the BSP tree) that takes up the whole size of the dungeon.
            BSPNode root = new BSPNode(new Rect(0, 0, rows, columns));
            
            // Recursively partition the dungeon space.
            Partition(root, minRoomSize, maxRoomSize);
            
            // Create rooms inside each node.
            root.CreateRooms();
            
            // Create an object that holds the dungeon data. 
            DungeonData data = new DungeonData();
            
            // Save data about the rooms and corridors of the dungeon.
            GetRooms(root, data.Rooms);
            GetCorridors(root, data.Corridors);
            
            return data;
        }

        /// <summary>
        /// Starting from the root node, recursively partitions the space into
        /// subspaces (sub-dungeons) until a leaf node is reached. 
        /// </summary>
        /// <param name="node">The subspace (BSP node) that will be partitioned.</param>
        /// <param name="minRoomSize">The minimum room size to partition the node into.</param>
        /// <param name="maxRoomSize">The maximum room size to partition the node into.</param>
        private void Partition(BSPNode node, int minRoomSize, int maxRoomSize)
        {
            // Check if node should be split (either too big or randomly decided)
            if (node.NodeBounds.width > maxRoomSize      
                || node.NodeBounds.height > maxRoomSize
                || Random.Range(0.0f, 1.0f) > SplitChanceThreshold) 
            {
                // If the sub-dungeon was successfully split, proceed to recursively partition its children
                if (node.Split(minRoomSize))
                {
                    Partition(node.LeftChild, minRoomSize, maxRoomSize);
                    Partition(node.RightChild, minRoomSize, maxRoomSize);
                }
            }
        }

        /// <summary>
        /// Recursively collects the rooms from all leaf nodes in a BSP tree.
        /// </summary>
        /// <param name="node">The current node to process.</param>
        /// <param name="rooms">The list to which all discovered rooms will be added.</param>
        private void GetRooms(BSPNode node, List<Rect> rooms)
        {
            if (node == null) return;

            if (node.IsLeaf())
            {
                Rect room = node.GetRoom();
                rooms.Add(room);
            }
            else
            {
                GetRooms(node.LeftChild, rooms);
                GetRooms(node.RightChild, rooms);
            }
        }

        /// <summary>
        /// Recursively collects all corridors from a BSP tree.
        /// </summary>
        /// <param name="node">The current node to process.</param>
        /// <param name="corridors">The list to which all discovered corridors will be added.</param>
        private void GetCorridors(BSPNode node, List<Rect> corridors)
        {
            if (node == null) return;
            
            corridors.AddRange(node.Corridors);
            
            GetCorridors(node.LeftChild, corridors);
            GetCorridors(node.RightChild, corridors);
        }
    }
}