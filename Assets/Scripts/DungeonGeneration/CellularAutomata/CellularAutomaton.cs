namespace DungeonGeneration.CellularAutomata
{
    public static class CellularAutomaton
    {
        // TODO extract all config values into a separate file
        
        // Defines the probability that a given cell will be 
        // a floor (i.e. alive) in the noise grid before smoothing.
        public static int Density = 55;
        
        // How many times the automaton will run.
        public static int Iterations = 5;

        /// <summary>
        /// Generates a grid filled randomly with 1's and 0'z
        /// depending on the density factor (i.e. probability).
        /// The resulting noise grid is the seed that will then be used
        /// to apply CA rules and achieve smoothing.
        /// </summary>
        /// <param name="width">The width (i.e. number of columns) of the grid.</param>
        /// <param name="height">The height (i.e. number of rows) of the grid.</param>
        /// <returns></returns>
        public static int[,] GenerateNoiseGrid(int width, int height)
        {
            // Create a new grid of size width x height. 
            // Each element inside that grid will be either
            // 1 (alive; floor) or 0 (dead; wall).
            int[,] noiseGrid = new int[width, height];

            System.Random random = new System.Random();

            // Iterate over grid.
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Determine whether an element will be 1 (floor) or 
                    // 0 (wall) by generating a random number and comparing it 
                    // against the probability factor (i.e. density).
                    noiseGrid[x, y] = random.Next(0, 100) < Density ? 1 : 0; 
                }
            }

            return noiseGrid;
        }
        
        public static int[,] ApplyRules(int[,] grid)
        {
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);

            for (int i = 0; i < Iterations; i++)
            {
                int[,] newGrid = new int[width, height];

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int wallNeighbors = CountNeighbors(grid, x, y);

                        if (grid[x, y] == 0) // wall
                            newGrid[x, y] = wallNeighbors >= 4 ? 0 : 1;
                        else // floor
                            newGrid[x, y] = wallNeighbors >= 5 ? 0 : 1;
                    }
                }

                grid = newGrid;
            }

            return grid;
        }

        private static int CountNeighbors(int[,] grid, int x, int y)
        {
            int count = 0;
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);

            for (int neighborX = x - 1; neighborX <= x + 1; neighborX++)
            {
                for (int neighborY = y - 1; neighborY <= y + 1; neighborY++)
                {
                    if (neighborX == x && neighborY == y) continue;

                    if (neighborX < 0 || neighborY < 0 || neighborX >= width || neighborY >= height)
                        count++;
                    else if (grid[neighborX, neighborY] == 0)
                        count++;
                }
            }

            return count;
        }
    }
}
