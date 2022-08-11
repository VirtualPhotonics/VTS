using System;

namespace Vts.Extensions
{
    /// <summary>
    /// Class for adding functionality to the .NET Random base class
    /// </summary>
    public static class RNGExtensions
    {
        /// <summary>
        /// Extension method to generate a random number within a specified range
        /// </summary>
        /// <param name="rng">The random number generator</param>
        /// <param name="minValue">The minimum value of range</param>
        /// <param name="maxValue">The maximum value of range</param>
        /// <returns>A double representing next random number in RNG sequence</returns>
        public static double NextDouble(this Random rng, double minValue, double maxValue)
        {
            double span = maxValue - minValue;

            return rng.NextDouble() * span + minValue;
        }

    }
}
