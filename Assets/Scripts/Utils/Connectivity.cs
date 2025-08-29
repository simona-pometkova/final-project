using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// A collection of methods related to ensuring
    /// connectivity (between rooms or otherwise).
    /// </summary>
    public static class Connectivity
    {
        /// <summary>
        /// Find all connected regions (islands) of floor tiles in a 2D integer grid.
        /// </summary>
        /// <param name="grid">The 2D integer array representing the room or map.</param>
        /// <returns>A list containing all regions (islands) in the grid.</returns>
        public static List<List<Vector2Int>> FindRoomIslands(int[,] grid)
        {
            // Store all discovered islands
            List<List<Vector2Int>> islands = new();
            
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            
            // Keep track of processed cells
            bool[,] visited = new bool[width, height];

            // Iterate over every cell in the grid
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (!visited[x, y] && grid[x, y] == 1)
                        // Perform a BFS Flood Fill starting from this cell 
                        // to find the entire connected region (island)
                        islands.Add(FloodFill.Run(grid, visited, x, y));

            return islands;
        }
        
        /// <summary>
        /// Connects all isolated floor regions (islands) in a 2D grid
        /// by carving corridors between them using Bresenham's Line.
        /// </summary>
        /// <param name="grid">The 2D integer array representing the room or map</param>
        /// <param name="islands">A list of floor regions (islands), where each region is a list of connected floor tiles.</param>
        public static void ConnectRoomIslands(int[,] grid, List<List<Vector2Int>> islands)
        {
            // Nothing needs to be connected
            if (islands.Count <= 1) return; 

            // Largest island is treated as the main one
            islands.Sort((a, b) => b.Count - a.Count);
            List<Vector2Int> mainIsland = islands[0];

            // Connect each smaller island to the main island
            for (int i = 1; i < islands.Count; i++)
            {
                List<Vector2Int> island = islands[i];

                Vector2Int closestTileMain = Vector2Int.zero;
                Vector2Int closestTileCurrent = Vector2Int.zero;
                
                int closestDistance = int.MaxValue;

                // Find the closest pair of tiles between main island and current island
                foreach (var floorTileMain in mainIsland)
                {
                    foreach (var floorTileCurrent in island)
                    {
                        int distance = Maths.ManhattanDistance(floorTileMain, floorTileCurrent);
                        
                        // Update shortest distance & floors
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestTileMain = floorTileMain;
                            closestTileCurrent = floorTileCurrent;
                        }
                    }
                }
                // Carve a corridor between the closest pair of tiles
                BresenhamLine.Draw(grid, closestTileMain, closestTileCurrent);
                
                // Merge current island into the main one
                mainIsland.AddRange(island);
            }
        }

        //TODO document
        public static List<Vector2Int> CollectFloorTiles(int[,] grid, Rect bounds)
        {
            List<Vector2Int> floorTiles = new();

            for (int localX = 0; localX < grid.GetLength(0); localX++)
            {
                for (int localY = 0; localY < grid.GetLength(1); localY++)
                {
                    if (grid[localX, localY] == 1)
                    {
                        int worldX = (int)bounds.x + localX;
                        int worldY = (int)bounds.y + localY;
                        
                        floorTiles.Add(new Vector2Int(worldX, worldY));
                    }
                }
            }

            return floorTiles;
        }
    }
}