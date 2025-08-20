using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGeneration.CellularAutomata
{
    public static class CellularAutomaton
    {
        public static int Density = 60;
        public static int Iterations = 20;

        public static int[,] GenerateNoiseGrid(int width, int height)
        {
            int[,] noiseGrid = new int[width, height];

            int density = CellularAutomaton.Density;
            System.Random random = new System.Random();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    noiseGrid[x, y] = random.Next(0, 100) < density ? 1 : 0; // 1: floor; 0: wall
                }
            }

            return noiseGrid;
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
    }
}
