using System;
using System.Linq;

namespace Vts.Common.Math
{
    /// <summary>
    /// Statistics utilities
    /// </summary>
    public static class Statistics
    {
        /// <summary>
        /// The method determines the discrete expected value given x and a probability density function, p(x)
        /// </summary>
        /// <param name="x">The double array of values</param>
        /// <param name="pOfX">The probability density function of x</param>
        /// <returns>An expected value or mean</returns>
        public static double ExpectedValue(double[] x, double[] pOfX)
        {
            return x.Zip(pOfX, (left, right) => (left * right)).Sum();
        }

        /// <summary>
        /// The method determines expected value of a 2D array given in 1D (row dominant) over the 2nd dimension 
        /// used to determine mean sampling depth
        /// </summary>
        /// <param name="array">The 2D array that has been flattened to a double array of values</param>
        /// <param name="x">The array of x values along 1 dimension</param>
        /// <param name="y">The array of y values along other dimension</param>
        /// <param name="dx">The array of delta x values</param>
        /// <param name="dy">The array delta y values</param>
        /// <returns>A double representing mean sampling depth</returns>
        public static double MeanSamplingDepth(double[] array, double[] x, double[] y, double[] dx, double[] dy)
        {
            if (array.Length != x.Length * y.Length)
                throw new ArgumentException("Dimensions of array must be dimension of x * dimension of y");
            
            var pdf = new double[y.Length];
            var ySum = 0D;
            for (var yi = 0; yi < y.Length; yi++)
            {
                for (var xi = 0; xi < x.Length; xi++)
                {
                    pdf[yi] += array[xi + yi * x.Length] * dx[xi];
                }
                pdf[yi] *= dy[yi];
                ySum += pdf[yi];
            }

            // normalize pdf
            for (var yi = 0; yi < y.Length; yi++)
            {
                pdf[yi] /= ySum;
            }
            return ExpectedValue(y, pdf);
        }
    }
}
