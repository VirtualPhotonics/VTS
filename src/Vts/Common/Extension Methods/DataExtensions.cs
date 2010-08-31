using System;
using System.Collections.Generic;

namespace Vts.Extensions
{
    /// <summary>
    /// Helper group of extension methods for manipulating data in arrays (e.g. adding noise, etc.)
    /// </summary>
    public static class DataExtensions
    {
        /// <summary>
        /// This extension method (static method "add-on" to the double[] class) adds noise to the input double array
        /// </summary>
        /// <param name="myDoubleArray"></param>
        /// <param name="percentNoise"></param>p
        /// <returns></returns>
        public static void AddNoise(this double[] myDoubleArray, double percentNoise)
        {
            // todo: make this functional/side-effect-free (return a new array with noise)
            double noiseFraction = percentNoise / 100.0;
            Random RandomNumber = new Random();

            for (int i = 0; i < myDoubleArray.Length; i++)
            {
                // Box Muller to get normal deviates mean=0 SD=1
                double uniformDeviate1 = RandomNumber.NextDouble();
                double uniformDeviate2 = RandomNumber.NextDouble();
                double gaussDeviate = Math.Sqrt(-2 * Math.Log(uniformDeviate1)) *
                    Math.Cos(2 * Math.PI * uniformDeviate2);
                myDoubleArray[i] *= 1 + noiseFraction * gaussDeviate;
            }
        }

        public static IEnumerable<double> AddNoise(this IEnumerable<double> myValues, double percentNoise)
        {
            // todo: make this functional/side-effect-free (return a new array with noise)
            double noiseFraction = percentNoise / 100.0;
            Random RandomNumber = new Random();

            foreach (double d in myValues)
            {
                // Box Muller to get normal deviates mean=0 SD=1
                double uniformDeviate1 = RandomNumber.NextDouble();
                double uniformDeviate2 = RandomNumber.NextDouble();
                double gaussDeviate = Math.Sqrt(-2 * Math.Log(uniformDeviate1)) *
                    Math.Cos(2 * Math.PI * uniformDeviate2);
                
                yield return (double) (d * (1 + noiseFraction * gaussDeviate));
            }

        }
    }
}
