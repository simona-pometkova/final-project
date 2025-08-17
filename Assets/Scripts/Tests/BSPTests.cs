using System.Collections.Generic;
using DungeonGeneration;
using DungeonGeneration.BinarySpacePartitioning;
using NUnit.Framework;
using UnityEngine;
using static DungeonGeneration.BinarySpacePartitioning.BSPNode;

namespace Tests
{
    public class BSPTests
    {
        /// <summary>
        /// Tests whether a node is successfully split.
        /// </summary>
        [Test]
        public void Node_IsSplit()
        {
            BSPNode root = new BSPNode(new Rect(0, 0, 64, 64));
            bool wasSplit = root.Split(16);

            Assert.IsTrue(wasSplit, "Node was not split.");
        }
        
        /// <summary>
        /// Tests whether a node has children
        /// (i.e. it is not a leaf) when it is split.
        /// </summary>
        [Test]
        public void Node_HasChildren()
        {
            BSPNode root = new BSPNode(new Rect(0, 0, 64, 64));
            root.Split(16);

            Assert.IsFalse(root.IsLeaf(), "Node does not have children.");
        }

        /// <summary>
        /// Tests whether a node is not split if it's too small, 
        /// i.e. it remains a leaf in the BSP tree and no children are created.
        /// </summary>
        [Test]
        public void Split_FailsWhenNodeTooSmall()
        {
            BSPNode root = new BSPNode(new Rect(0, 0, 10, 10));
            bool didSplit = root.Split(16);

            Assert.IsFalse(didSplit, "Node was split.");
        }

        /// <summary>
        /// Tests whether a left child is created
        /// when splitting a node.
        /// </summary>
        [Test]
        public void Split_CreatesLeftChild()
        {
            BSPNode root = new BSPNode(new Rect(0, 0, 64, 64));
            root.Split(16);
            
            Assert.IsNotNull(root.LeftChild, "Left child does not exist.");
        }

        /// <summary>
        /// Tests whether a right child is created
        /// when splitting a node.
        /// </summary>
        [Test]
        public void Split_CreatesRightChild()
        {
            BSPNode root = new BSPNode(new Rect(0, 0, 64, 64));
            root.Split(16);
            
            Assert.IsNotNull(root.RightChild, "Right child does not exist.");
        }

        /// <summary>
        /// Tests whether rooms fail to be created
        /// if dungeon dimensions are too small.
        /// </summary>
        [Test]
        public void Dungeon_NoRoomsIfDungeonTooSmall()
        {
            DungeonGenerator generator = new DungeonGenerator(1, 1, 8, 16);
            generator.GenerateDungeon();
            
            Assert.IsEmpty(generator.Dungeon.Rooms, "No rooms should be generated when dungeon is too small.");
        }

        /// <summary>
        /// Tests whether corridors fail to be created
        /// if dungeon dimensions are too small.
        /// </summary>
        [Test]
        public void Dungeon_NoCorridorsIfDungeonTooSmall()
        {
            DungeonGenerator generator = new DungeonGenerator(1, 1, 8, 16);
            generator.GenerateDungeon();
            
            Assert.IsEmpty(generator.Dungeon.Corridors, "No corridors should be generated when dungeon is too small.");
        }

        /// <summary>
        /// Tests whether rooms count matches leaf nodes count.
        /// Each leaf node should contain a room.
        /// </summary>
        [Test]
        public void Dungeon_RoomsCountMatchesLeavesCount()
        {
            BSPNode root = new BSPNode(new Rect(0, 0, 32, 32));
            root.CreateRooms();
            List<BSPNode> leaves = root.GetLeafNodes();
            int roomsCount = 0;

            foreach (var leaf in leaves)
            {
                var room = leaf.GetRoom();
                if (room != null) roomsCount++;
                
                Assert.IsTrue(room != null, "Leaf node has no room.");
            }
                
            Assert.AreEqual(leaves.Count, roomsCount, "Some leaves do not have a room.");
        }

        /// <summary>
        /// Tests whether any corridors inside the dungeon are duplicated.
        /// </summary>
        [Test]
        public void Corridors_NoDuplicates()
        {
            DungeonGenerator generator = new DungeonGenerator(50, 50, 10, 15);
            generator.GenerateDungeon();
            
            HashSet<string> seen = new HashSet<string>();

            foreach (var corridor in generator.Dungeon.Corridors)
            {
                string id = $"{corridor.x}, {corridor.y}, {corridor.width}, {corridor.height}";
                Assert.IsTrue(seen.Add(id), $"Duplicate corridor was found: {id}");
            }
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
            Room room = node.GetRoom();

            Assert.IsTrue(node.NodeBounds.Overlaps(room.Bounds), "Room does not fit inside node bounds.");
        }
    }
}

