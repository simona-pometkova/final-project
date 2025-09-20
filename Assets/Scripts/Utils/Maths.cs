using UnityEngine;

namespace Utils
{
    /// <summary>
    /// A collection of mathematics formulas used throughout the codebase.
    /// </summary>
    public static class Maths
    {
        /// <summary>
        /// Calculate and return the absolute distance between two points.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns></returns>
        public static int ManhattanDistance(Vector2Int a, Vector2Int b) => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        
        /// <summary>
        /// Calculate and return the largest value.
        /// </summary>
        /// <param name="a">First number.</param>
        /// <param name="b">Second number.</param>
        /// <returns>The larger number between the two.</returns>
        public static int Max(int a, int b) => Mathf.Max(a, b);
        
        /// <summary>
        /// Calculate and return the smallest value.
        /// </summary>
        /// <param name="a">First number.</param>
        /// <param name="b">Second number.</param>
        /// <returns>The smaller number between the two.</returns>
        public static float Min(float a, float b) => Mathf.Min(a, b);
        
        /// <summary>
        /// Get a random float number ranging between two numbers.
        /// </summary>
        /// <param name="a">First number.</param>
        /// <param name="b">Second number.</param>
        /// <returns>A random number between the two.</returns>
        public static float GetRandomFloat(float a, float b) => Random.Range(a, b);
        
        /// <summary>
        /// Get a random integer number ranging between two numbers.
        /// </summary>
        /// <param name="a">First number.</param>
        /// <param name="b">Second number.</param>
        /// <returns>A random number between the two.</returns>
        public static int GetRandomInt(int a, int b) => Random.Range(a, b);
    }
}