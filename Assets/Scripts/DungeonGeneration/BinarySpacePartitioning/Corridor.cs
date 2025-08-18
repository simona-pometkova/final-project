using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static DungeonGeneration.BinarySpacePartitioning.BSPNode;

namespace DungeonGeneration.BinarySpacePartitioning
{
    public class Corridor
    {
        public List<Rect> Segments => _segments;
        public Vector2Int Start { get; }
        public Vector2Int End { get; }

        private List<Rect> _segments = new();

        private const int CorridorThickness = 1;

        public Corridor(Vector2Int start, Vector2Int end)
        {
            Start = start;
            End = end;

            GenerateSegments();
        }

        private void GenerateSegments()
        {
            // Horizontal segment
            if (Start.x != End.x)
            {
                int left = Mathf.Min(Start.x, End.x);
                int width = Mathf.Abs(End.x - Start.x) + CorridorThickness;
                _segments.Add(new Rect(left, Start.y, width, CorridorThickness));
            }

            // Vertical segment
            if (Start.y != End.y)
            {
                int bottom = Mathf.Min(Start.y, End.y);
                int height = Mathf.Abs(End.y - Start.y) + CorridorThickness;
                _segments.Add(new Rect(End.x, bottom, CorridorThickness, height));
            }
        }
    }
}
