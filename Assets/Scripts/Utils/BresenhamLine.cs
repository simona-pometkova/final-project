using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Implementation of the Bresenham's Line algorithm.
    /// Used for carving corridors and other connections throughout the dungeon.
    /// </summary>
    public static class BresenhamLine
    {
        /// <summary>
        /// Draws a straight line between two points in a 2D grid.
        /// Optionally, the line can have a specified thickness.
        /// </summary>
        /// <param name="grid">The 2D integer array representing the room or map.</param>
        /// <param name="start">The starting coordinate of the line.</param>
        /// <param name="end">The ending coordinate of the line.</param>
        /// <param name="thickness">Optional - thickness of the line (measured in cells).</param>
        public static void Draw(int[,] grid, Vector2Int start, Vector2Int end, int thickness = 3)
        {
            int radius = Maths.Max(0, (thickness - 1) / 2);
            
            // Start and end coordinates
            int x0 = start.x;
            int y0 = start.y;
            int x1 = end.x;
            int y1 = end.y;

            // Absolute differences
            int dx = Mathf.Abs(x1 - x0);
            int dy = Mathf.Abs(y1 - y0);
            
            // Step direction for X and Y
            int stepX = x0 < x1 ? 1 : -1;
            int stepY = y0 < y1 ? 1 : -1;
            
            // Error term for Bresenham's algorithm
            int error = dx - dy;

            // Loop until we reach the end point
            while (true)
            {
                // Draw the current point with thickness
                DrawLineThick(grid, x0, y0, radius);

                // End if the end point has been reached
                if (x0 == x1 && y0 == y1) break;

                int doubleError = 2 * error;
                
                // Adjust term and move along x-axis if necessary
                if (doubleError > -dy)
                {
                    error -= dy;
                    x0 += stepX;
                }
                
                // Adjust term and move along y-axis if necessary 
                if (doubleError < dx)
                {
                    error += dx;
                    y0 += stepY;
                }
            }
        }
        
        /// <summary>
        /// Draws a filled circular area (disk) on a 2D grid centered at a given coordinate.
        /// </summary>
        /// <param name="grid">The 2D integer array representing the room or map.</param>
        /// <param name="circleX">The x-coordinate of the circle's center.</param>
        /// <param name="circleY">The y-coordinate of the circle's center.</param>
        /// <param name="radius">The radius of the circle in cells. Determines the "thickness" of the line.</param>
        private static void DrawLineThick(int[,] grid, int circleX, int circleY, int radius)
        {
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            
            // Iterate over a square bounding box centered at (cx, cy) with side length 2*r + 1
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    // Absolute coordinates
                    int nx = circleX + dx;
                    int ny = circleY + dy;

                    // Skip cells outside grid boundaries
                    if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                        // If cell is within circle, mark it as floor
                        if (dx * dx + dy * dy <= radius * radius) 
                            grid[nx, ny] = 1; // floor
                }
            }
        }
    }
}