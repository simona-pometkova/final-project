using System.Collections.Generic;
using UnityEngine;

namespace DungeonGeneration.BinarySpacePartitioning
{
    public class Corridor
    {
        public List<Rect> Segments => _segments;
        public Room RightRoom { get; private set; }
        public Room LeftRoom { get; private set;}

        private List<Rect> _segments = new();

        private const int CorridorThickness = 1;
        private const int CorridorPadding = 5;

        public Corridor(Room leftRoom, Room rightRoom)
        {
            this.LeftRoom = leftRoom;
            this.RightRoom = rightRoom;

            Vector2Int leftPoint = new(
                (int)Random.Range(leftRoom.Bounds.xMin + CorridorPadding, leftRoom.Bounds.xMax - CorridorPadding),
                (int)Random.Range(leftRoom.Bounds.yMin + CorridorPadding, leftRoom.Bounds.yMax - CorridorPadding)
            );

            Vector2Int rightPoint = new(
                (int)Random.Range(rightRoom.Bounds.xMin + CorridorPadding, rightRoom.Bounds.xMax - CorridorPadding),
                (int)Random.Range(rightRoom.Bounds.yMin + CorridorPadding, rightRoom.Bounds.yMax - CorridorPadding)
            );

            GenerateSegments(leftPoint, rightPoint);
        }

        private void GenerateSegments(Vector2Int leftPoint, Vector2Int rightPoint)
        {
            // Horizontal segment
            if (leftPoint.x != rightPoint.x)
            {
                int left = Mathf.Min(leftPoint.x, rightPoint.x);
                int width = Mathf.Abs(rightPoint.x - leftPoint.x) + CorridorThickness;
                _segments.Add(new Rect(left, leftPoint.y, width, CorridorThickness));
            }

            // Vertical segment
            if (leftPoint.y != rightPoint.y)
            {
                int bottom = Mathf.Min(leftPoint.y, rightPoint.y);
                int height = Mathf.Abs(rightPoint.y - leftPoint.y) + CorridorThickness;
                _segments.Add(new Rect(rightPoint.x, bottom, CorridorThickness, height));
            }
        }
    }
}
