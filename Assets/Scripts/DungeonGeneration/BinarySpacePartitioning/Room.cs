using System.Collections.Generic;
using DungeonGeneration.CellularAutomata;
using UnityEngine;
using Utils;

namespace DungeonGeneration.BinarySpacePartitioning
{
    /// <summary>
    /// Workaround non-nullable structs. Defining the room
    /// as its own class allows me to not have a room inside a node
    /// if I don't need to (e.g. node is too small to be partitioned).
    /// </summary>
    public class Room
    {
        // Position and dimensions of the room.
        public Rect Bounds { get; }
        
        // The local room grid of wall/floor cells.
        // Used by Cellular Automata for smoothing.
        public int[,] Grid { get; }
        
        public List<Vector2Int> FloorTiles { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">Start position of the room on the x-axis.</param>
        /// <param name="y">Start position of the room on the y-axis.</param>
        /// <param name="width">Width of the room (number of 1x1 tiles).</param>
        /// <param name="height">Height of the room (number of 1x1 tiles).</param>
        public Room(float x, float y, int width, int height)
        {
            this.Bounds = new Rect(x, y, width, height);

            // Apply local CA smoothing to the room:
            // // 1. Generate a noise grid
            // // 2. Smooth out using CA rules
            this.Grid = CellularAutomaton.GenerateNoiseGrid(width, height);
            this.Grid = CellularAutomaton.ApplyRules(this.Grid);

            // Post-process CA smoothing to connect isolated islands of floor
            var islands = Connectivity.FindRoomIslands(this.Grid);
            Connectivity.ConnectRoomIslands(this.Grid, islands);
            this.FloorTiles = Connectivity.CollectFloorTiles(this.Grid, this.Bounds);
        }

        /// <summary>
        /// Project the room's floor tiles onto
        /// the global dungeon grid.
        /// </summary>
        /// <param name="dungeonGrid">The 2D integer array representing the dungeon grid.</param>
        public void TranslateToGlobalGrid(int[,] dungeonGrid)
        {
            foreach (var tile in this.FloorTiles)
                if (tile.x >= 0 && tile.x < dungeonGrid.GetLength(0)
                                && tile.y >= 0 && tile.y < dungeonGrid.GetLength(1))
                    // Mark as floor (1)
                    dungeonGrid[tile.x, tile.y] = 1; 
        }
    }
}
