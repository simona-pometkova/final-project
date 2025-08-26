
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Implementation of the Flood Fill algorithm
    /// using BFS (Breadth-First Search). 
    /// </summary>
    public static class FloodFill
    {
        /// <summary>
        /// Used to check all four neighbors (von Neumann neighborhood)
        /// of a given grid cell/coordinate.
        /// </summary>
        private static readonly (int x, int y)[] Directions =
        {
            (1, 0), // right
            (-1, 0), // left
            (0, 1), // up
            (0, -1) // down
        };

        /// <summary>
        /// Performs a Breadth-First Search (BFS) flood fill on a
        /// 2D integer grid starting from a given coordinate.
        /// </summary>
        /// <param name="grid">The 2D integer grid (1 - floor, 0 - wall).</param>
        /// <param name="visited">A boolean 2D array tracking which cells have already been processed.</param>
        /// <param name="startX">The x-coordinate from which to start the BFS.</param>
        /// <param name="startY">The y-coordinate from which to start the BFS.</param>
        /// <returns>A list of coordinates (i.e. a region) that belong to the same connected region.</returns>
        public static List<Vector2Int> Run(int[,] grid, bool[,] visited, int startX, int startY)
        {
            // Store all coordinates that belong to the current region
            List<Vector2Int> region = new();

            int width = grid.GetLength(0);
            int height = grid.GetLength(1);

            // Initialize BFS traversal
            Queue<Vector2Int> queue = new();
            queue.Enqueue(new Vector2Int(startX, startY));
            visited[startX, startY] = true;

            // Perform BFS until all reachable floor tiles are processed
            while (queue.Count > 0)
            {
                Vector2Int point = queue.Dequeue();
                region.Add(point);

                // Check all neighbors (von Neumann neighborhood) of current cell
                foreach (var direction in Directions)
                {
                    int neighborX = point.x + direction.x;
                    int neighborY = point.y + direction.y;

                    // Skip neighbors that are out of grid boundaries
                    if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                    {
                        // If neighbor is a floor and hasn't been visited, enqueue it
                        if (!visited[neighborX, neighborY] && grid[neighborX, neighborY] == 1)
                        {
                            visited[neighborX, neighborY] = true;
                            queue.Enqueue(new Vector2Int(neighborX, neighborY));
                        }
                    }
                }
            }
            // Return all coordinates of the connected region
            return region;
        }
    }
}
