using System;
using System.Linq;
using Vts.Extensions;

namespace Vts.Common.Math
{
    public static class Statistics
    {
        /// <summary>
        /// Statistics utilities
        /// </summary>
        /// 
        /// method determines the discrete expected value given x and a probability density function, p(x)
        public static double ExpectedValue(double[] x, double[] pOfX)
        {
            return EnumerableEx.Zip(x, pOfX, (left, right) => (left * right)).Sum();
        }

        /// method determines expected value of a 2D array given in 1D (row dominant) over the 2nd dimension 
        /// used to determine mean sampling depth
        public static double MeanSamplingDepth(double[] array, double[] x1, double[] x2)
        {
            if (array.Length != x1.Length * x2.Length)
                throw new ArgumentException("Dimensions of array must be dimension of x1 * dimension of x2");

            //double[] pdf = Enumerable.Range(0, x2.Length).Select(i => array.Skip(i * x1.Length).Take(x1.Length).Sum() / x1.Length).ToArray();
            double[] pdf = new double[x2.Length];
            for (int i = 0; i < x2.Length; i++)
            {
                  pdf[i] = array.Skip(i * x1.Length).Take(x1.Length).Sum();
            }
            // normalize pdf
            var sum = pdf.Sum(); // otherwise it keeps looping when you call Sum() inside...and your data's changing...
            for (int i = 0; i < x2.Length; i++)
            {
                pdf[i] /= sum;
                //pdf[i] /= pdf.Sum();
            }
            return ExpectedValue(x2, pdf);
        }
    }
}
