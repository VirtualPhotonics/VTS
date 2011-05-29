using System;
using Vts.IO;
using System.Runtime.Serialization;

namespace Vts.Extensions
{
    [KnownType(typeof(Random))]
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
        /// <summary>
        /// methods to save current state of random number generator
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Random FromFile(string filename)
        {
            return FileIO.ReadFromXML<Random>(filename);
        }

        public static void ToFile(this Random rng, string filename)
        {
            FileIO.WriteToXML(rng, filename);
        }
    }
}
