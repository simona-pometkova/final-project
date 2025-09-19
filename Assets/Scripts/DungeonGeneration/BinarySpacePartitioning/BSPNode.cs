using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace DungeonGeneration.BinarySpacePartitioning
{
    /// <summary>
    /// Represents a node in the Binary Space Partitioning (BSP) tree used to divide the dungeon space.
    /// Each node either holds a rectangular room (leaf) or is split into two child nodes.
    /// The BSP tree structure is used to recursively subdivide space, generate rooms, and connect them with corridors.
    /// </summary>
    public class BSPNode
    {
        // Getters
        public BSPNode LeftChild { get; private set; }
        public BSPNode RightChild { get; private set; }
        public Rect NodeBounds { get; }
        
        private Room _room;

        private const int RoomEdgePadding = 1;
        private const int RoomSizeMargin = 2;
        private const float AspectRatioThreshold = 1.25f;
        private const float SplitDirectionThreshold = 0.5f;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="nodeBounds">The dimensions of the node.</param>
        public BSPNode(Rect nodeBounds)
        {
            this.NodeBounds = nodeBounds;
        }

        /// <summary>
        /// Checks if the node is a leaf in the BSP tree, i.e. whether it has any children.
        /// </summary>
        /// <returns>True if the node has no children; false otherwise.</returns>
        public bool IsLeaf() => this.LeftChild == null && this.RightChild == null;

        /// <summary>
        /// Attempts to split the current BSP node into two child nodes, either horizontally or vertically,
        /// based on the dimensions of the current space and room size constraints.
        /// </summary>
        /// <param name="minRoomSize">The minimum allowable size of a room in either dimension.</param>
        /// <returns>True if the split was successful and child nodes were created; false otherwise.</returns>
        public bool Split(int minRoomSize)
        {
            // Already split
            if (!IsLeaf()) return false;

            // Choose a vertical or horizontal split depending on the proportions.
            // I.e. if too wide split vertically, if too long split horizontally, 
            // or if approximately square - choose vertical or horizontal at random
            bool splitHorizontally;

            if (this.NodeBounds.width / this.NodeBounds.height >= AspectRatioThreshold)
                splitHorizontally = false;
            else if (this.NodeBounds.height / this.NodeBounds.width >= AspectRatioThreshold)
                splitHorizontally = true;
            else
                splitHorizontally = Maths.GetRandomFloat(0.0f, 1.0f) > SplitDirectionThreshold;

            // Too small - don't split
            if (Maths.Min(this.NodeBounds.height, this.NodeBounds.width) / 2 < minRoomSize) return false;

            if (splitHorizontally)
            {
                // Split so that the resulting sub-dungeons widths are not too small
                int split = Maths.GetRandomInt(minRoomSize, (int)(this.NodeBounds.width - minRoomSize));

                this.LeftChild = new BSPNode(new Rect(this.NodeBounds.x, this.NodeBounds.y, this.NodeBounds.width, split));
                this.RightChild = new BSPNode(
                    new Rect(this.NodeBounds.x, this.NodeBounds.y + split, this.NodeBounds.width, this.NodeBounds.height - split));
            }
            else // Split vertically
            {
                int split = Maths.GetRandomInt(minRoomSize, (int)(this.NodeBounds.height - minRoomSize));

                this.LeftChild = new BSPNode(new Rect(this.NodeBounds.x, this.NodeBounds.y, split, this.NodeBounds.height));
                this.RightChild = new BSPNode(new Rect(this.NodeBounds.x + split, this.NodeBounds.y, this.NodeBounds.width - split, this.NodeBounds.height));
            }

            return true;
        }

        /// <summary>
        /// Recursively creates rooms in each node of the BSP tree.
        /// </summary>
        public void CreateRooms()
        {
            // If the node has children, create rooms inside them too
            this.LeftChild?.CreateRooms();
            this.RightChild?.CreateRooms();

            // Ready to hold a room - create one
            if (IsLeaf())
            {
                int roomWidth = Maths.GetRandomInt((int)this.NodeBounds.width / 2, (int)this.NodeBounds.width - RoomSizeMargin);
                int roomHeight = Maths.GetRandomInt((int)this.NodeBounds.height / 2, (int)this.NodeBounds.height - RoomSizeMargin);
                int roomX = Maths.GetRandomInt(RoomEdgePadding, (int)this.NodeBounds.width - roomWidth - RoomEdgePadding);
                int roomY = Maths.GetRandomInt(RoomEdgePadding, (int)this.NodeBounds.height - roomHeight - RoomEdgePadding);

                // Room position will be absolute in the board, not relative to the sub-dungeon
                // Only create a room if dimensions are big enough
                if (roomWidth != 0 && roomHeight != 0)
                    _room = new Room(this.NodeBounds.x + roomX, this.NodeBounds.y + roomY, roomWidth, roomHeight);
            }
        }

        /// <summary>
        /// Recursively creates corridors by connecting
        /// sibling rooms in the BSP tree.
        /// </summary>
        /// <param name="dungeonGrid">The grid to carve corridors onto.</param>
        public void CreateCorridors(int[,] dungeonGrid)
        {
            // If the node has children, create corridors
            // recursively in their subtrees too
            this.LeftChild?.CreateCorridors(dungeonGrid);
            this.RightChild?.CreateCorridors(dungeonGrid);

            if (this.LeftChild != null && this.RightChild != null)
            {
                Room leftRoom = this.LeftChild.GetRoom();
                Room rightRoom = this.RightChild.GetRoom();
            
                // Connect sibling rooms
                if (leftRoom != null && rightRoom != null)
                    Connectivity.ConnectRooms(dungeonGrid, leftRoom.FloorTiles, rightRoom.FloorTiles);
            }
        }
        
        /// <summary>
        /// Recursively retrieves the first valid room contained
        /// within this BSP node or its children.
        /// </summary>
        /// <returns>The first valid room found in this node or its children. Null room if no room exists.</returns>
        public Room GetRoom()
        {
            if (IsLeaf()) return _room;

            if (this.LeftChild != null)
            {
                Room leftRoom = this.LeftChild.GetRoom();
                if (leftRoom != null) return leftRoom;
            }

            if (this.RightChild != null)
            {
                Room rightRoom = this.RightChild.GetRoom();
                if (rightRoom != null) return rightRoom;
            }

            // No room is found
            return null;
        }
    }
}

