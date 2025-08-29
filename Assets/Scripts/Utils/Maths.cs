using UnityEngine;

namespace Utils
{
    /// <summary>
    /// A collection of mathematics formulas used throughout the codebase.
    /// </summary>
    public static class Maths
    {
        /// <summary>
        /// Calculate and return the absolute distance between two points in a grid.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns></returns>
        public static int ManhattanDistance(Vector2Int a, Vector2Int b) => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        
        //TODO document
        public static int Max(int a, int b) => Mathf.Max(a, b);
    }
}