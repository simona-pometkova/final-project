using UnityEngine;
using Utils;

namespace DungeonGeneration.BinarySpacePartitioning
{
    public class Corridor
    {
        public Room LeftRoom { get; }
        public Room RightRoom { get; }

        private const int CorridorThickness = 3;
        public Corridor(Room leftRoom, Room rightRoom, int[,] dungeonGrid)
        {
            this.LeftRoom = leftRoom;
            this.RightRoom = rightRoom;
            
            var(start, end) = FindClosestTiles();
            
            //TODO document and extract in separate method
            Vector2Int corner = new Vector2Int(end.x, start.y);
            
            // Straight line
            // BresenhamLine.Draw(dungeonGrid, start, end, 2, CorridorThickness);
            
            // L-shaped
            if (Random.value < 0.5f)
            {
                BresenhamLine.Draw(dungeonGrid, start, corner, 2, CorridorThickness);
                BresenhamLine.Draw(dungeonGrid, corner, end, 2, CorridorThickness);
            }
            else
            {
                BresenhamLine.Draw(dungeonGrid, start, corner, 2, CorridorThickness);
                BresenhamLine.Draw(dungeonGrid, corner, end, 2, CorridorThickness);
            }
        }

        //TODO document
        //TODO extract in utils assembly?
        private (Vector2Int a, Vector2Int b) FindClosestTiles()
        {
            Vector2Int closestA = Vector2Int.zero;
            Vector2Int closestB = Vector2Int.zero;
            int bestDist = int.MaxValue;
            
            foreach (var tileA in LeftRoom.FloorTiles)
            {
                foreach (var tileB in RightRoom.FloorTiles)
                {
                    int dist = Maths.ManhattanDistance(tileA, tileB);
                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        closestA = tileA;
                        closestB = tileB;
                    }
                }
            }

            return (closestA, closestB);
        }
    }
}
