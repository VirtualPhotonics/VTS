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
        /// <param name="rng"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static double NextDouble(this Random rng, double minValue, double maxValue)
        {
            double span = maxValue - minValue;

            return rng.NextDouble() * span + minValue;
        }

    }
}
