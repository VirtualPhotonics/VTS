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
        /// method determines the discrete expected value given x and a probability density function, p(x)
        /// </summary>
        /// <param name="x">double array of values</param>
        /// <param name="pOfX">probability density function of x</param>
        /// <returns>expected value or mean</returns>
        public static double ExpectedValue(double[] x, double[] pOfX)
        {
            return Enumerable.Zip(x, pOfX, (left, right) => (left * right)).Sum();
        }

        /// <sumary>
        /// method determines expected value of a 2D array given in 1D (row dominant) over the 2nd dimension 
        /// used to determine mean sampling depth
        /// </sumary>
        /// <param name="array">2D array that has been flattened to a double array of values</param>
        /// <param name="x">2D array x values along 1 dimension</param>
        /// <param name="y">2D array y values along other dimension</param>
        /// <param name="dx">delta x values</param>
        /// <param name="dy">delta y values</param>
        /// <returns>double representing mean sampling depth</returns>
        public static double MeanSamplingDepth(double[] array, double[] x, double[] y, double[] dx, double[] dy)
        {
            if (array.Length != x.Length * y.Length)
                throw new ArgumentException("Dimensions of array must be dimension of x * dimension of y");
            
            double[] pdf = new double[y.Length];
            double ySum = 0D;
            for (int yi = 0; yi < y.Length; yi++)
            {
                for (int xi = 0; xi < x.Length; xi++)
                {
                    pdf[yi] += array[xi + yi * x.Length] * dx[xi];
                }
                pdf[yi] *= dy[yi];
                ySum += pdf[yi];
            }

            // normalize pdf
            for (int yi = 0; yi < y.Length; yi++)
            {
                pdf[yi] /= ySum;
            }
            return ExpectedValue(y, pdf);
        }
    }
}
