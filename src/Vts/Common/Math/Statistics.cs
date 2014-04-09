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
        public static double ExpectedValue(double[] x, double[] pOfX)
        {
            return Enumerable.Zip(x, pOfX, (left, right) => (left * right)).Sum();
        }

        /// <sumary>
        /// method determines expected value of a 2D array given in 1D (row dominant) over the 2nd dimension 
        /// used to determine mean sampling depth
        /// </sumary>
        public static double MeanSamplingDepth(double[] array, double[] x, double[] y, double[] dx, double[] dy)
        {
            if (array.Length != x.Length * y.Length)
                throw new ArgumentException("Dimensions of array must be dimension of x * dimension of y");

            //double[] pdf = Enumerable.Range(0, x2.Length).Select(i => array.Skip(i * x1.Length).Take(x1.Length).Sum() / x1.Length).ToArray();
            double[] pdf = new double[y.Length];
            double ySum = 0D;
            for (int yi = 0; yi < y.Length; yi++)
            {
                //pdf[i] = array.Skip(i * x1.Length).Take(x1.Length).Sum();
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
