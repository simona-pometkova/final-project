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

                // Find the closest pair of tiles between the main island and the current island
                (Vector2Int closestTileMain, Vector2Int closestTileCurrent) = FindClosestTiles(mainIsland, island);
                
                // Carve a corridor between the closest pair of tiles
                BresenhamLine.Draw(grid, closestTileMain, closestTileCurrent);
                
                // Merge current island into the main one
                mainIsland.AddRange(island);
            }
        }
        
        /// <summary>
        /// Collects all floor tiles (1) from a grid and converts
        /// their local coordinates into world coordinates based on the given bounds.
        /// </summary>
        /// <param name="grid">The 2D integer array representing the local grid.</param>
        /// <param name="bounds">The rectangular bounds of the grid in world space.</param>
        /// <returns>A list of positions representing the world coordinates of all discovered floor tiles.</returns>
        public static List<Vector2Int> CollectFloorTiles(int[,] grid, Rect bounds)
        {
            List<Vector2Int> floorTiles = new();

            // Iterate over local grid
            for (int localX = 0; localX < grid.GetLength(0); localX++)
                for (int localY = 0; localY < grid.GetLength(1); localY++)
                    // Coordinate is a floor
                    if (grid[localX, localY] == 1) 
                    {
                        // Convert to world coordinates and add to list
                        int worldX = (int)bounds.x + localX;
                        int worldY = (int)bounds.y + localY;
                        
                        floorTiles.Add(new Vector2Int(worldX, worldY));
                    }

            return floorTiles;
        }

        /// <summary>
        /// Connect two rooms by carving an L-shaped corridor
        /// between their closest floor tiles.
        /// </summary>
        /// <param name="dungeonGrid">The 2D integer array representing the dungeon. Modified in place.</param>
        /// <param name="leftRoomTiles">A list of positions representing the floor tiles of the first room.</param>
        /// <param name="rightRoomTiles">A list of positions representing the floor tiles of the second room.</param>
        public static void ConnectRooms(int[,] dungeonGrid, List<Vector2Int> leftRoomTiles,
            List<Vector2Int> rightRoomTiles)
        {
            // Find the closest pair tiles between the two rooms
            (Vector2Int start, Vector2Int end) = FindClosestTiles(leftRoomTiles, rightRoomTiles);

            // Pick an L-shaped corner (horizontal then vertical or vertical then horizontal)
            Vector2Int corner;
            if (Random.value < 0.5f)
                // Horizontal first
                corner = new Vector2Int(end.x, start.y); 
            else
                // Vertical first
                corner = new Vector2Int(start.x, end.y); 

            BresenhamLine.Draw(dungeonGrid, start, corner);
            BresenhamLine.Draw(dungeonGrid, corner, end);
        }
        
        /// <summary>
        /// Finds the closest pair of floor tiles between
        /// two sets of tiles using Manhattan distance.
        /// </summary>
        /// <param name="tilesA">A list of positions representing the first set of tiles.</param>
        /// <param name="tilesB">A list of positions representing the second set of tiles.</param>
        /// <returns>A tuple containing the two closest tiles from the first and second set, respectively.</returns>
        private static (Vector2Int a, Vector2Int b) FindClosestTiles(List<Vector2Int> tilesA, List<Vector2Int> tilesB)
        {
            Vector2Int closestTileA = Vector2Int.zero;
            Vector2Int closestTileB = Vector2Int.zero;
            
            int closestDistance = int.MaxValue;
            
            // Iterate over tiles
            foreach (var tileA in tilesA)
                foreach (var tileB in tilesB)
                {
                    // Calculate distance 
                    int distance = Maths.ManhattanDistance(tileA, tileB);
                    
                    // Update if shorter distance was found
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestTileA = tileA;
                        closestTileB = tileB;
                    }
                }

            return (closestTileA, closestTileB);
        }
    }
}