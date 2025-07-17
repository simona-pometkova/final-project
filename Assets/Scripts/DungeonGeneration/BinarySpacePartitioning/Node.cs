using System.Collections.Generic;
using UnityEngine;

namespace DungeonGeneration.BinarySpacePartitioning
{
    /// <summary>
    /// Represents a node in the Binary Space Partitioning (BSP) tree used to divide the dungeon space.
    /// Each node either holds a rectangular room (leaf) or is split into two child nodes (internal).
    /// The BSP tree structure is used to recursively subdivide space, generate rooms, and connect them with corridors.
    /// </summary>
    public class Node
    {
        // Getters
        public Node LeftChild => _leftChild;
        public Node RightChild => _rightChild;
        public Rect Rect => _rect;
        public Rect Room => _room;
        public List<Rect> Corridors => _corridors;
        
        private Node _leftChild, _rightChild;
        private Rect _rect;
        private Rect _room = new Rect(-1, -1, 0, 0); // I.e. null
        private List<Rect> _corridors = new();

        // Configurable constants
        private const int RoomEdgePadding = 1;
        private const int RoomSizeMargin = 2;
        private const int CorridorBoundaryPadding = 1;

        private const float AspectRatioThreshold = 1.25f;
        private const float SplitDirectionThreshold = 0.5f;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rect">The dimensions of the node.</param>
        public Node(Rect rect)
        {
            this._rect = rect;
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

            if (_rect.width / _rect.height >= AspectRatioThreshold)
                splitHorizontally = false;
            else if (_rect.height / _rect.width >= AspectRatioThreshold)
                splitHorizontally = true;
            else
                splitHorizontally = Random.Range(0.0f, 1.0f) > SplitDirectionThreshold;

            // Too small - don't split
            if (Mathf.Min(_rect.height, _rect.width) / 2 < minRoomSize) return false;

            if (splitHorizontally)
            {
                // Split so that the resulting sub-dungeons widths are not too small
                int split = Random.Range(minRoomSize, (int)(_rect.width - minRoomSize));

                _leftChild = new Node(new Rect(_rect.x, _rect.y, _rect.width, split));
                _rightChild = new Node(
                    new Rect(_rect.x, _rect.y + split, _rect.width, _rect.height - split));
            }
            else // Split vertically
            {
                int split = Random.Range(minRoomSize, (int)(_rect.height - minRoomSize));

                _leftChild = new Node(new Rect(_rect.x, _rect.y, split, _rect.height));
                _rightChild = new Node(
                    new Rect(_rect.x + split, _rect.y, _rect.width - split, _rect.height));
            }

            return true;
        }

        /// <summary>
        /// Recursively creates rooms in each node of the BSP tree,
        /// and connects sibling nodes with corridors.
        /// </summary>
        public void CreateRoom()
        {
            // If the node has children, create rooms inside them too
            _leftChild?.CreateRoom();
            _rightChild?.CreateRoom();

            // If both children exist, create a connection (corridor) between them
            if (_leftChild != null && _rightChild != null)
                CreateCorridorBetween(_leftChild, _rightChild);

            // Ready to hold a room - create one
            if (IsLeaf())
            {
                int roomWidth = (int)Random.Range(_rect.width / 2, _rect.width - RoomSizeMargin);
                int roomHeight = (int)Random.Range(_rect.height / 2, _rect.height - RoomSizeMargin);
                int roomX = (int)Random.Range(1, _rect.width - roomWidth - RoomEdgePadding);
                int roomY = (int)Random.Range(1, _rect.height - roomHeight - RoomEdgePadding);

                // Room position will be absolute in the board, not relative to the sub-dungeon
                _room = new Rect(_rect.x + roomX, _rect.y + roomY, roomWidth, roomHeight);
            }
        }
        
        /// <summary>
        /// Creates an L-shaped corridor between two rooms represented by the given BSP nodes.
        /// A random point is selected inside each room (with padding from the walls),
        /// and a corridor is drawn between them. If the rooms are aligned vertically, a straight
        /// vertical corridor is created.
        /// </summary>
        /// <param name="left">The BSP node representing the first room.</param>
        /// <param name="right">The BSP node representing the second room.</param>
        private void CreateCorridorBetween(Node left, Node right)
        {
            Rect leftRoom = left.GetRoom();
            Rect rightRoom = right.GetRoom();

            // Attach the corridor to a random point in each room
            Vector2 leftPoint = new Vector2((int)Random.Range(leftRoom.x + CorridorBoundaryPadding, leftRoom.xMax - CorridorBoundaryPadding),
                (int)Random.Range(leftRoom.y + CorridorBoundaryPadding, leftRoom.yMax - CorridorBoundaryPadding));
            Vector2 rightPoint = new Vector2((int)Random.Range(rightRoom.x + CorridorBoundaryPadding, rightRoom.xMax - CorridorBoundaryPadding),
                (int)Random.Range(rightRoom.y + CorridorBoundaryPadding, rightRoom.yMax - CorridorBoundaryPadding));

            // Ensure leftPoint is to the left of rightPoint to simplify horizontal corridor logic
            if (leftPoint.x > rightPoint.x)
                //Swap via deconstruction.
                (leftPoint, rightPoint) = (rightPoint, leftPoint);

            int width = (int)(leftPoint.x - rightPoint.x);
            int height = (int)(leftPoint.y - rightPoint.y);

            // If the points are not aligned horizontally
            if (width != 0)
            {
                // Choose at random to go horizontal then vertical or the opposite
                if (Random.Range(0, 1) > 2)
                {
                    // Add a corridor to the right
                    _corridors.Add(new Rect(leftPoint.x, leftPoint.y, Mathf.Abs(width) + 1, 1));

                    // If left point is below right point, go up; otherwise go down
                    _corridors.Add(height < 0
                        ? new Rect(rightPoint.x, leftPoint.y, 1, Mathf.Abs(height)) 
                        : new Rect(rightPoint.x, leftPoint.y, 1, -Mathf.Abs(height)));
                }
                else
                {
                    // Go up or down
                    _corridors.Add(height < 0
                        ? new Rect(leftPoint.x, leftPoint.y, 1, Mathf.Abs(height))
                        : new Rect(leftPoint.x, rightPoint.y, 1, Mathf.Abs(height)));

                    // Go right
                    _corridors.Add(new Rect(leftPoint.x, rightPoint.y, Mathf.Abs(width) + 1, 1));
                }
            }
            else
            {
                // If the points are aligned horizontally, go up or down depending on the positions
                _corridors.Add(height < 0
                    ? new Rect((int)leftPoint.x, (int)leftPoint.y, 1, Mathf.Abs(height))
                    : new Rect((int)rightPoint.x, (int)rightPoint.y, 1, Mathf.Abs(height)));
            }
        }
        
        /// <summary>
        /// Recursively retrieves the first valid room (Rect) contained within this BSP node or its children.
        /// </summary>
        /// <returns>The first valid room Rect found in this node or its children. Null room if no rooms exist.</returns>
        private Rect GetRoom()
        {
            if (IsLeaf()) return _room;

            if (_leftChild != null)
            {
                Rect leftRoom = _leftChild.GetRoom();
                if (!Mathf.Approximately(leftRoom.x, -1)) return leftRoom;
            }

            if (_rightChild != null)
            {
                Rect rightRoom = _rightChild.GetRoom();
                if (!Mathf.Approximately(rightRoom.x, -1)) return rightRoom;
            }

            // No room is found - workaround non-nullable struct
            return new Rect(-1, -1, 0, 0);
        }
    }
}

