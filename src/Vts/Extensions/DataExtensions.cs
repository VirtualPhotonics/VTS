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
        /// <param name="myDoubleArray">The values to which noise will be added</param>
        /// <param name="percentNoise">The double indicating percent noise to be added
        /// percentNoise=10 means 10% noise will be added</param>
        /// <returns>A double array of values with noise added</returns>
        public static double[] AddNoise(this double[] myDoubleArray, double percentNoise)
        {
            // make this functional/side-effect-free (return a new array with noise)?
            var noiseFraction = percentNoise / 100.0;
            var randomNumber = new Random();

            for (var i = 0; i < myDoubleArray.Length; i++)
            {
                // Box Muller to get normal deviates mean=0 SD=1
                var uniformDeviate1 = randomNumber.NextDouble();
                var uniformDeviate2 = randomNumber.NextDouble();
                var gaussDeviate = Math.Sqrt(-2 * Math.Log(uniformDeviate1)) *
                                   Math.Cos(2 * Math.PI * uniformDeviate2);
                myDoubleArray[i] *= 1 + noiseFraction * gaussDeviate;
            }
            return myDoubleArray;
        }
        /// <summary>
        /// Static method to add percentage noise given IEnumerable of double values.
        /// </summary>
        /// <param name="myValues">IEnumerable list of values</param>
        /// <param name="percentNoise">The double designating percent, percentNoise=10 means 10 percent
        /// noise added</param>
        /// <returns>An IEnumerable of double with values with noise added</returns>
        public static IEnumerable<double> AddNoise(this IEnumerable<double> myValues, double percentNoise)
        {
            // make this functional/side-effect-free (return a new array with noise)?
            var noiseFraction = percentNoise / 100.0;
            var randomNumber = new Random();

            foreach (var d in myValues)
            {
                // Box Muller to get normal deviates mean=0 SD=1
                var uniformDeviate1 = randomNumber.NextDouble();
                var uniformDeviate2 = randomNumber.NextDouble();
                var gaussDeviate = Math.Sqrt(-2 * Math.Log(uniformDeviate1)) *
                                   Math.Cos(2 * Math.PI * uniformDeviate2);
                
                yield return  (d * (1 + noiseFraction * gaussDeviate));
            }

        }
    }
}
