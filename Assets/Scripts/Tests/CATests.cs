using DungeonGeneration.CellularAutomata;
using NUnit.Framework;

namespace Tests
{
    /// <summary>
    /// A collection of unit tests focused on the
    /// Cellular Automata algorithm.
    /// </summary>
    public class CATests
    {
        /// <summary>
        /// Tests whether all cells in a noise grid
        /// are walls if density = 0.
        /// </summary>
        [Test]
        public void CA_AllCellsAreWalls()
        {
            int width = 50;
            int height = 50;
            int[,] grid = CellularAutomaton.GenerateNoiseGrid(width, height, 0);

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    Assert.IsTrue(grid[x, y] == 0, "Cell should be a wall.");
        }

        /// <summary>
        /// Tests whether all cells in a noise grid
        /// are floors if density = 100.
        /// </summary>
        [Test]
        public void CA_AllCellsAreFloors()
        {
            int width = 50;
            int height = 50;
            
            int[,] grid = CellularAutomaton.GenerateNoiseGrid(width, height, 100);

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    Assert.IsTrue(grid[x, y] == 1, "Cell should be a floor.");
        }

        /// <summary>
        /// Tests whether GenerateNoiseGrid returns
        /// a grid with the correct dimensions.
        /// </summary>
        [Test]
        public void CA_NoiseGridHasCorrectDimensions()
        {
            int width = 25;
            int height = 50;
            int[,] grid = CellularAutomaton.GenerateNoiseGrid(width, height);
            
            Assert.AreEqual(width, grid.GetLength(0), "Grid width is incorrect.");
            Assert.AreEqual(height, grid.GetLength(1), "Grid height is incorrect.");
        }
    }
}

