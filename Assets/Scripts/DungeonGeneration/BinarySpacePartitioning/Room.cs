using DungeonGeneration.CellularAutomata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DungeonGeneration.BinarySpacePartitioning
{
    /// <summary>
    /// Workaround non-nullable structs. Defining the room
    /// as its own class allows me to not have a room inside a node
    /// if I don't need to (e.g. node is too small to be partitioned).
    /// </summary>
    public class Room
    {
        // Used to define the position and shape 
        // of the room - instead of the room directly
        // being a Rect. 
        public Rect Bounds { get; }
        public int[,] NoiseGrid => _noiseGrid;

        private int[,] _noiseGrid;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">Start position of the room on the x-axis.</param>
        /// <param name="y">Start position of the room on the y-axis.</param>
        /// <param name="width">Width of the room (number of tiles).</param>
        /// <param name="height">Height of the room (number of tiles).</param>
        public Room(float x, float y, int width, int height)
        {
            this.Bounds = new Rect(x, y, width, height);

            this._noiseGrid = CellularAutomaton.GenerateNoiseGrid(width, height);
            this._noiseGrid = CellularAutomaton.ApplyRules(this._noiseGrid);
        }
    }
}
