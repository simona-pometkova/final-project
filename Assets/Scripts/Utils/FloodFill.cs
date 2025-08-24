
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class FloodFill
    {
        private static readonly (int x, int y)[] Directions =
        {
            (1, 0), (-1, 0), (0, 1), (0, -1)
        };

        public static List<List<Vector2Int>> FindRoomIslands(int[,] grid)
        {
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);

            bool[,] visited = new bool[width, height];
            List<List<Vector2Int>> islands = new();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (!visited[x, y] && grid[x, y] == 1) // floor
                    {
                        List<Vector2Int> island = new();
                        Queue<Vector2Int> queue = new();
                        queue.Enqueue(new Vector2Int(x, y));
                        visited[x, y] = true;

                        while(queue.Count > 0)
                        {
                            Vector2Int point = queue.Dequeue();
                            island.Add(point);

                            foreach (var dir in Directions)
                            {
                                int neighborX = point.x + dir.x;
                                int neighborY = point.y + dir.y;

                                if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                                {
                                    if (!visited[neighborX, neighborY] && grid[neighborX, neighborY] == 1)
                                    {
                                        visited[neighborX, neighborY] = true;
                                        queue.Enqueue(new Vector2Int(neighborX, neighborY));
                                    }
                                }
                            }
                        }
                        islands.Add(island);
                    }
                }
            }
            return islands;
        }
    }
}
