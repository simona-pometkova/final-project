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

        /// <summary>
        /// Tests whether a uniform wall (0's) grid
        /// remains all walls after applying CA rules.
        /// </summary>
        [Test]
        public void CA_AllWallsStayWalls()
        {
            int[,] noiseGrid = new int[5, 5]; // all walls (0's)
            int[,] result = CellularAutomaton.ApplyRules(noiseGrid);
            
            for (int x = 0; x < 5; x++)
                for (int y = 0; y < 5; y++)
                    Assert.AreEqual(0, result[x, y], "A noise grid with all walls should result in a grid with all walls.");
        }

        /// <summary>
        /// Tests whether a uniform floor (1's) grid
        /// remains all floors (interior cells only)
        /// after smoothing. Out-of-bounds neighbors
        /// are treated as walls.
        /// </summary>
        [Test]
        public void CA_AllFloors_InteriorStayFloors()
        {
            int[,] noiseGrid = new int[10, 10];
            
            for (int x = 0; x < 10; x++)
                for (int y = 0; y < 10; y++)
                    noiseGrid[x, y] = 1; // floor
            
            int[,] result = CellularAutomaton.ApplyRules(noiseGrid);
            
            for (int x = 1; x < 9; x++)
                for (int y = 1; y < 9; y++)
                    Assert.AreEqual(1, result[x, y], "A noise grid with all floors should result in a grid where all interior cells are floors.");
        }

        /// <summary>
        /// Tests the Survival Limit rule:
        /// A wall with too few wall neighbors should become a floor.
        /// Out-of-bounds neighbors are treated as walls.
        /// </summary>
        [Test]
        public void CA_SurvivalLimit_WallBecomesFloor()
        {
            // Tests the center cell (coords [2,2])
            int[,] noiseGrid =
            {
                { 1, 1, 1, 1, 1},
                { 1, 1, 1, 0, 1},
                { 1, 1, 0, 1, 1},
                { 1, 0, 1, 1, 1},
                { 1, 1, 1, 1, 1}
            };
            
            int[,] result = CellularAutomaton.ApplyRules(noiseGrid, 1);
            
            Assert.AreEqual(1, result[2, 2], "Wall with too few wall neighbors should become a floor.");
        }

        /// <summary>
        /// Tests the Survival Limit rule:
        /// A wall with enough wall neighbors should remain a wall.
        /// </summary>
        [Test]
        public void CA_SurvivalLimit_WallStaysWall()
        {
            // Tests the center cell (coords [2,2])
            int[,] noiseGrid =
            {
                { 0, 0, 0, 0, 1},
                { 1, 0, 0, 1, 1},
                { 1, 0, 0, 1, 0},
                { 1, 0, 0, 0, 0},
                { 1, 0, 1, 1, 1}
            };
            
            int[,] result = CellularAutomaton.ApplyRules(noiseGrid, 1);

            Assert.AreEqual(0, result[2, 2], "Wall with enough wall neighbors should remain a wall.");
        }

        /// <summary>
        /// Tests the Birth Limit rule:
        /// A floor with too many wall neighbors should become a wall.
        /// </summary>
        [Test]
        public void CA_BirthLimit_FloorBecomesWall()
        {
            // Tests the center cell (coords [2,2])
            int[,] noiseGrid =
            {
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 1, 0 },
                { 0, 0, 1, 0, 0 },
                { 0, 1, 0, 0, 0 },
                { 0, 0, 0, 0, 0 }
            };
            
            int[,] result = CellularAutomaton.ApplyRules(noiseGrid, 1);
            
            Assert.AreEqual(0, result[2, 2], "Floor with too many wall neighbors should become a wall.");
        }

        /// <summary>
        /// Tests the Birth Limit rule:
        /// A floor with enough floor neighbors should remain a floor.
        /// Out-of-bounds neighbors are treated as walls.
        /// </summary>
        [Test]
        public void CA_BirthLimit_FloorStaysFloor()
        {
            // Tests the center cell (coords [2,2])
            int[,] noiseGrid =
            {
                { 0, 0, 0, 0, 0},
                { 0, 1, 1, 0, 0},
                { 0, 1, 1, 1, 0},
                { 0, 0, 1, 1, 0},
                { 0, 0, 0, 0, 0}
            };
            
            int[,] result = CellularAutomaton.ApplyRules(noiseGrid, 1);
            
            Assert.AreEqual(1, result[2, 2], "Floor with enough floor neighbors should remain a floor.");
        }
    }
}

