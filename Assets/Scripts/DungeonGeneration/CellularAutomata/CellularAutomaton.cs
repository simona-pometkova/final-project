namespace DungeonGeneration.CellularAutomata
{
    /// <summary>
    /// Provides functionality for applying a Cellular Automaton
    /// to binary dungeon grids (0 = wall, 1 = floor).
    /// Inspired by rules similar to Conway's Game of Life, this automaton
    /// smooths out random noise into more natural cave-like structures.
    /// The Cellular Automaton is applied locally to each BSP room
    /// after Binary Space Partitioning has generated the overall layout.
    /// </summary>
    public static class CellularAutomaton
    {
        // TODO extract all config values into a separate file and serialize them
        // Defines the probability that a given cell will be 
        // a floor (i.e. 1/alive) in the noise grid before smoothing
        // Density = % chance floor
        private const int Density = 55;
        
        // How many times the automaton will run
        private const int Iterations = 5;

        // 4/5 rule
        private const int BirthLimit = 5;
        private const int SurvivalLimit = 4;

        /// <summary>
        /// Generates a random binary grid (2D array) of walls and floors,
        /// which serves as the initial seed for the Cellular Automaton.
        /// 
        /// Each cell is assigned as a floor (1) with probability equal to
        /// <see cref="Density"/>, otherwise as a wall (0).
        /// 
        /// This random noise grid is then used as input for smoothing.
        /// </summary>
        /// <param name="width">Number of columns in the grid.</param>
        /// <param name="height">Number of rows in the grid.</param>
        /// <param name="density">Probability factor. Defaults to <see cref="Density"/>.</param>
        /// <returns>
        /// A 2D array where 1 represents a floor cell and 0 represents a wall cell.
        /// </returns>
        public static int[,] GenerateNoiseGrid(int width, int height, int density = Density)
        {
            // Initialize an empty grid of the given size
            int[,] noiseGrid = new int[width, height];

            // Use a random generator for assigning floor/wall states
            System.Random random = new System.Random();

            // Fill the grid with random floors/walls based on Density
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    // Floor (1) if roll is below Density threshold, otherwise wall (0)
                    noiseGrid[x, y] = random.Next(0, 100) < density ? 1 : 0; 

            return noiseGrid;
        }
        
        /// <summary>
        /// Applies a Cellular Automaton smoothing step to the noise grid.
        /// Runs for a fixed number of iterations (see <see cref="Iterations"/>).
        /// 
        /// On each iteration, the algorithm:
        /// 1. Counts the number of wall neighbors for each cell (Moore neighbourhood).
        /// 2. Applies the following rules:
        ///    - If a cell is a wall (0), it stays a wall if it has 
        ///      at least <see cref="SurvivalLimit"/> wall neighbors; 
        ///      otherwise it becomes a floor (1).
        ///    - If a cell is a floor (1), it becomes a wall if it has 
        ///      at least <see cref="BirthLimit"/> wall neighbors; 
        ///      otherwise it remains a floor.
        /// 
        /// This gradually removes isolated cells and smooths the layout 
        /// into cave-like structures.
        /// </summary>
        /// <param name="grid">The binary grid (0 = wall, 1 = floor) to process.</param>
        /// <param name="iterations">How many iterations the cellular automaton will run.</param>
        /// <returns>
        /// A new grid after applying the Cellular Automaton rules for 
        /// the configured number of iterations.
        /// </returns>
        public static int[,] ApplyRules(int[,] grid, int iterations = Iterations)
        {
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);

            // Repeat the smoothing process for the configured number of iterations
            for (int i = 0; i < iterations; i++)
            {
                // Use a new grid each iteration so updates don’t affect neighbor checks mid-step
                int[,] newGrid = new int[width, height];

                // Evaluate each cell in the grid
                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                    {
                        int wallNeighbors = CountWallNeighbors(grid, x, y);

                        // Rule application:
                        // // - Walls survive if surrounded by enough walls (>= SurvivalLimit)
                        // // - Floors die if too many walls are around (>= BirthLimit)
                        if (grid[x, y] == 0) // current cell is a wall
                            newGrid[x, y] = wallNeighbors >= SurvivalLimit ? 0 : 1;
                        else // current cell is a floor
                            newGrid[x, y] = wallNeighbors >= BirthLimit ? 0 : 1;
                    }

                // Replace the noise grid with the updated one for the next iteration
                grid = newGrid;
            }

            // Return the fully smoothed grid
            return grid;
        }

        /// <summary>
        /// Counts the number of wall neighbors (0) surrounding a given cell.
        /// Uses an 8-way neighborhood (Moore neighborhood).
        /// 
        /// Out-of-bounds positions are treated as walls to ensure that 
        /// the outer boundary of the grid behaves like a solid border.
        /// </summary>
        /// <param name="grid">The binary grid (0 = wall, 1 = floor).</param>
        /// <param name="x">The x-coordinate of the target cell.</param>
        /// <param name="y">The y-coordinate of the target cell.</param>
        /// <returns>The number of neighboring wall cells (0–8).</returns>
        private static int CountWallNeighbors(int[,] grid, int x, int y)
        {
            int count = 0;
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);

            // Check all 8 neighbors around (x, y)
            for (int neighborX = x - 1; neighborX <= x + 1; neighborX++)
            {
                for (int neighborY = y - 1; neighborY <= y + 1; neighborY++)
                {
                    // Skip itself
                    if (neighborX == x && neighborY == y) continue;

                    // Out-of-bound cells count as walls
                    if (neighborX < 0 || neighborY < 0 || neighborX >= width || neighborY >= height) 
                        count++;
                    
                    // Inside bounds: increment if the neighbor is a wall
                    else if (grid[neighborX, neighborY] == 0) // wall
                        count++;
                }
            }

            return count;
        }
    }
}
