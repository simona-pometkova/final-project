using DungeonGeneration;
using DungeonGeneration.BinarySpacePartitioning;
using NUnit.Framework;
using UnityEngine;

public class BSPTests
{
    /// <summary>
    /// Tests whether a node is successfully split.
    /// </summary>
    [Test]
    public void Node_SplitsSuccessfully()
    {
        BSPNode root = new BSPNode(new Rect(0, 0, 64, 64));
        bool didSplit = root.Split(16);

        Assert.IsTrue(didSplit, "Node split was unsuccessful.");
    }

    /// <summary>
    /// Tests whether a node is split into two children nodes,
    /// i.e. it is not a leaf node.
    /// </summary>
    [Test]
    public void Node_CreatesTwoChildren()
    {
        BSPNode root = new BSPNode(new Rect(0, 0, 64, 64));
        root.Split(16);

        Assert.IsNotNull(root.LeftChild, "Left child is null.");
        Assert.IsNotNull(root.RightChild, "Right child is null.");
        Assert.IsFalse(root.IsLeaf(), "Node is a leaf and does not contain children.");
    }

    /// <summary>
    /// Tests whether a node is not split if it's too small, 
    /// i.e. it remains a leaf in the BSP tree and no children are created.
    /// </summary>
    [Test]
    public void Node_SplitFailsWhenTooSmall()
    {
        BSPNode root = new BSPNode(new Rect(0, 0, 10, 10));
        bool didSplit = root.Split(16);

        Assert.IsFalse(didSplit, "Node was split.");
        Assert.IsTrue(root.IsLeaf(), "Node is not a leaf.");
        Assert.IsNull(root.LeftChild, "Node has a left child.");
        Assert.IsNull(root.RightChild, "Node has a right child.");
    }

    /// <summary>
    /// Tests whether a room fits inside the bounds of a node,
    /// i.e. a room is not bigger than the node it is located in.
    /// </summary>
    [Test]
    public void Room_IsWithinNodeBounds()
    {
        BSPNode node = new BSPNode(new Rect(0, 0, 32, 32));
        node.CreateRooms();
        Rect room = node.GetRoom();

        Assert.IsTrue(room.width > 0 && room.height > 0, "Room is nonexistent.");
        Assert.IsTrue(node.NodeBounds.Contains(new Vector2(room.xMin, room.yMin)), "Room is out of node bounds.");
        Assert.IsTrue(node.NodeBounds.Contains(new Vector2(room.xMax, room.yMax)), "Room is out of node bounds.");
    }

    /// <summary>
    /// Tests whether the algorithm creates a room 
    /// inside each leaf node of the BSP tree.
    /// </summary>
    [Test]
    public void Room_CreatesRoomInAllLeaves()
    {
        BSPNode root = new BSPNode(new Rect(0, 0, 64, 64));
        root.Split(16);
        root.LeftChild.Split(16);
        root.RightChild.Split(16);

        root.CreateRooms();

        var leaves = root.GetLeafNodes();

        foreach (var leaf in leaves)
        {
            Rect room = leaf.GetRoom();
            Assert.IsTrue(room.width > 0, "Leaf node has no room.");
        }
    }

    /// <summary>
    /// Tests whether the dungeon fails to be generated
    /// (i.e. no rooms and corridors are created)
    /// if its dimensions are too small.
    /// </summary>
    [Test]
    public void DungeonGenerator_HandlesTooSmallDungeon()
    {
        DungeonGenerator generator = new DungeonGenerator(1, 1, 8, 16);
        generator.GenerateDungeon();
        DungeonData data = generator.GetData();
        
        Assert.IsEmpty(data.Rooms, "No rooms should be generated when dungeon is too small.");
        Assert.IsEmpty(data.Corridors, "No corridors should be generated when dungeon is too small.");
    } 
}
