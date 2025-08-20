using System.Collections.Generic;
using UnityEngine;

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
        public BSPNode LeftChild => _leftChild;
        public BSPNode RightChild => _rightChild;
        public Rect NodeBounds => _nodeBounds;
        public List<Corridor> Corridors => _corridors;
        
        private BSPNode _leftChild, _rightChild;
        private Rect _nodeBounds;
        private Room _room;
        private List<Corridor> _corridors = new();

        // Configurable constants
        // TODO export as Serializable fields
        private const int RoomEdgePadding = 1;
        private const int RoomSizeMargin = 2;

        private const float AspectRatioThreshold = 1.25f;
        private const float SplitDirectionThreshold = 0.5f;
        private const int CorridorPadding = 5;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="nodeBounds">The dimensions of the node.</param>
        public BSPNode(Rect nodeBounds)
        {
            this._nodeBounds = nodeBounds;
        }

        /// <summary>
        /// Checks if the node is a leaf in the BSP tree, i.e. whether it has any children.
        /// </summary>
        /// <returns>True if the node has no children; false otherwise.</returns>
        public bool IsLeaf()
        {
            return _leftChild == null && _rightChild == null;
        }

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

            if (_nodeBounds.width / _nodeBounds.height >= AspectRatioThreshold)
                splitHorizontally = false;
            else if (_nodeBounds.height / _nodeBounds.width >= AspectRatioThreshold)
                splitHorizontally = true;
            else
                splitHorizontally = Random.Range(0.0f, 1.0f) > SplitDirectionThreshold;

            // Too small - don't split
            if (Mathf.Min(_nodeBounds.height, _nodeBounds.width) / 2 < minRoomSize) return false;

            if (splitHorizontally)
            {
                // Split so that the resulting sub-dungeons widths are not too small
                int split = Random.Range(minRoomSize, (int)(_nodeBounds.width - minRoomSize));

                _leftChild = new BSPNode(new Rect(_nodeBounds.x, _nodeBounds.y, _nodeBounds.width, split));
                _rightChild = new BSPNode(
                    new Rect(_nodeBounds.x, _nodeBounds.y + split, _nodeBounds.width, _nodeBounds.height - split));
            }
            else // Split vertically
            {
                int split = Random.Range(minRoomSize, (int)(_nodeBounds.height - minRoomSize));

                _leftChild = new BSPNode(new Rect(_nodeBounds.x, _nodeBounds.y, split, _nodeBounds.height));
                _rightChild = new BSPNode(new Rect(_nodeBounds.x + split, _nodeBounds.y, _nodeBounds.width - split, _nodeBounds.height));
            }

            return true;
        }

        /// <summary>
        /// Recursively creates rooms in each node of the BSP tree,
        /// and connects sibling nodes with corridors.
        /// </summary>
        public void CreateRooms()
        {
            // If the node has children, create rooms inside them too
            _leftChild?.CreateRooms();
            _rightChild?.CreateRooms();

            // If both children exist, connect them with a corridor
            // TODO removed corridors for now. Will connect rooms after
            // CA terrain smoothing is implemented.

            //if (_leftChild != null && _rightChild != null)
            //{
            //    Room leftRoom = _leftChild.GetRoom();
            //    Room rightRoom = _rightChild.GetRoom();

            //    if (leftRoom != null && rightRoom != null)
            //    {
            //        Corridor corridor = new Corridor(leftRoom, rightRoom);
            //        _corridors.Add(corridor);
            //    }
            //}

            // Ready to hold a room - create one
            if (IsLeaf())
            {
                int roomWidth = (int)Random.Range(_nodeBounds.width / 2, _nodeBounds.width - RoomSizeMargin);
                int roomHeight = (int)Random.Range(_nodeBounds.height / 2, _nodeBounds.height - RoomSizeMargin);
                int roomX = (int)Random.Range(RoomEdgePadding, _nodeBounds.width - roomWidth - RoomEdgePadding);
                int roomY = (int)Random.Range(RoomEdgePadding, _nodeBounds.height - roomHeight - RoomEdgePadding);

                // Room position will be absolute in the board, not relative to the sub-dungeon
                // Only create a room if dimensions are big enough
                if (roomWidth != 0 && roomHeight != 0)
                    _room = new Room(_nodeBounds.x + roomX, _nodeBounds.y + roomY, roomWidth, roomHeight);
            }
        }
        
        /// <summary>
        /// Recursively retrieves the first valid room contained within this BSP node or its children.
        /// </summary>
        /// <returns>The first valid room found in this node or its children. Null room if no room exists.</returns>
        public Room GetRoom()
        {
            if (IsLeaf()) return _room;

            if (_leftChild != null)
            {
                Room leftRoom = _leftChild.GetRoom();
                if (leftRoom != null) return leftRoom;
            }

            if (_rightChild != null)
            {
                Room rightRoom = _rightChild.GetRoom();
                if (rightRoom != null) return rightRoom;
            }

            // No room is found
            return null;
        }

        /// <summary>
        /// Recursively traverses the BSP tree and returns all leaf nodes.
        /// </summary>
        /// <returns>A list containing all leaf nodes in the BSP subtree rooted at this node.</returns>
        public List<BSPNode> GetLeafNodes()
        {
            List<BSPNode> leaves = new();

            // If this node is a leaf, add it to the list.
            if (IsLeaf()) leaves.Add(this);
            else
            {
                // This node is not a leaf - traverse its children and get the leaf nodes inside of them.
                if (_leftChild != null) leaves.AddRange(_leftChild.GetLeafNodes());
                if (_rightChild != null) leaves.AddRange(_rightChild.GetLeafNodes());
            }

            return leaves;
        }
    }
}

