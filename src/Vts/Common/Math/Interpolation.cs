using System;
using System.Collections.Generic;

namespace Vts.Common.Math
{
    /// <summary>
    /// Provides methods for interpolation of data in 1D and 2D
    /// </summary>
    public static class Interpolation
    {
        #region Single-value interpolation (float and double only)
        /// <summary>
        /// Interpolation in one dimension (assumes x are monotonically increasing)
        /// </summary>
        /// <param name="x">The known dependent values</param>
        /// <param name="y">The known independent values</param>
        /// <param name="xi">Value to at which to interpolate</param>
        /// <returns>If xi outside range of x, returns NaN,
        ///     otherwide, returns linearly interpolated result</returns>
        public static double interp1(IList<double> x, IList<double> y, double xi)
        {
            if (x.Count != y.Count)
            {
                throw new ArgumentException("Error in interp1: arrays x and y are not the same size!");
            }

            int currentIndex = 1;

            // changed this to clip to bounds (DC - 7/26/09)
            if ((xi < x[0]))
                return y[0];
            else if((xi > x[x.Count - 1]))
                return y[y.Count - 1];
            else
            {
                // increment the index until you pass the desired interpolation point
                while (x[currentIndex] < xi) currentIndex++;

                // then do the interp between x[currentIndex-1] and xi[currentIndex]
                double t = (xi - x[currentIndex - 1]) / (x[currentIndex] - x[currentIndex - 1]);
                return y[currentIndex - 1] + t * (y[currentIndex] - y[currentIndex - 1]);
            }
        }

        /// <summary>
        /// Interpolation in one dimension (assumes x are monotonically increasing)
        /// </summary>
        /// <param name="x">The known dependent values</param>
        /// <param name="y">The known independent values</param>
        /// <param name="xi">Value to at which to interpolate</param>
        /// <returns>If xi outside range of x, returns NaN,
        ///     otherwide, returns linearly interpolated result</returns>
        public static float interp1(IList<float> x, IList<float> y, float xi)
        {
            int currentIndex = 1;

            if ((xi < x[0]) || (xi > x[x.Count - 1])) return float.NaN;
            else
            {
                // increment the index until you pass the desired interpolation point
                while (x[currentIndex] < xi) currentIndex++;

                // then do the interp between x[currentIndex-1] and xi[currentIndex]
                float t = (xi - x[currentIndex - 1]) / (x[currentIndex] - x[currentIndex - 1]);
                return y[currentIndex - 1] + t * (y[currentIndex] - y[currentIndex - 1]);
            }
        }

        /// <summary>
        /// Interpolation in one dimension (assumes x are monotonically increasing) of 2D array 
        /// over either 1st or 2nd dimension with fixed index in other dimension 
        /// </summary>
        /// <param name="x">The known dependent values</param>
        /// <param name="y">The known independent values</param>
        /// <param name="xi">Value to at which to interpolate</param>
        /// <param name="fixedDimension">Dimension of 2D array to keep fixed</param>
        /// <param name="fixedIndex">Fixed index of dim</param>
        /// <returns>The interpolated value (clamped to boundary values if xi are of range)</returns>
        public static double interp1(IList<double> x, double[,] y, double xi, int fixedDimension, int fixedIndex)
        {
            double[] temp;
            switch (fixedDimension)
            {
                case 1:  // interpolate over 2nd dimension
                    temp = new double[y.GetLength(1)];
                    for (int i = 0; i < y.GetLength(1); i++)
                    {
                        temp[i] = y[fixedIndex, i];
                    }
                    break;
                default:
                case 2:
                    temp = new double[y.GetLength(0)];
                    for (int i = 0; i < y.GetLength(0); i++)
                    {
                        temp[i] = y[i, fixedIndex];
                    }

                    break;
            }
            return interp1(x, temp, xi);
        }

        /// <summary>
        /// Interpolation in one dimension (assumes x are monotonically increasing) of 2D array 
        /// over either 1st or 2nd dimension with fixed index in other dimension 
        /// </summary>
        /// <param name="x">The known dependent values</param>
        /// <param name="y">The known independent values</param>
        /// <param name="xi">Value to at which to interpolate</param>
        /// <param name="fixedDimension">Dimension of 2D array to keep fixed</param>
        /// <param name="fixedIndex">Fixed index of dim</param>
        /// <returns>The interpolated value (clamped to boundary values if xi are of range)</returns>
        public static float interp1(IList<float> x, float[,] y, float xi, int fixedDimension, int fixedIndex)
        {
            float[] temp;
            switch (fixedDimension)
            {
                case 1:  // interpolate over 2nd dimension
                    temp = new float[y.GetLength(1)];
                    for (int i = 0; i < y.GetLength(1); i++)
                    {
                        temp[i] = y[fixedIndex, i];
                    }
                    break;
                default:
                case 2:
                    temp = new float[y.GetLength(0)];
                    for (int i = 0; i < y.GetLength(0); i++)
                    {
                        temp[i] = y[i, fixedIndex];
                    }

                    break;
            }
            return interp1(x, temp, xi);
        }

        #endregion

        #region Multi-value interpolation

        // todo: "flip" vectorized and scalar implementations to remove inefficiency 

        /// <summary>
        /// Interpolation in one dimension (assumes x are monotonically increasing)
        /// </summary>
        /// <param name="x">The known dependent values</param>
        /// <param name="y">The known independent values</param>
        /// <param name="xs">Value to at which to interpolate</param>
        /// <returns>If xs outside range of x, returns NaN,
        ///     otherwide, returns linearly interpolated result</returns>
        public static IEnumerable<double> interp1(IList<double> x, IList<double> y, IEnumerable<double> xs)
        {
            foreach (var xi in xs)
            {
                yield return interp1(x, y, xi);
            }
        }
        /// <summary>
        /// Interpolation in one dimension (assumes x are monotonically increasing)
        /// </summary>
        /// <param name="x">The known dependent values</param>
        /// <param name="y">The known independent values</param>
        /// <param name="xs">Value to at which to interpolate</param>
        /// <returns>If xs outside range of x, returns NaN,
        ///     otherwide, returns linearly interpolated result</returns>
        public static IEnumerable<float> interp1(IList<float> x, IList<float> y, IEnumerable<float> xs)
        {
            foreach (var xi in xs)
            {
                yield return interp1(x, y, xi);
            }
        }

        public static IEnumerable<double> interp1(IList<double> x, double[,] y, IEnumerable<double> xs, int fixedDimension, int fixedIndex)
        {
            foreach (var xi in xs)
            {
                yield return interp1(x, y, xi, fixedDimension, fixedIndex);
            }
        }

        public static IEnumerable<float> interp1(IList<float> x, float[,] y, IEnumerable<float> xs, int fixedDimension, int fixedIndex)
        {
            foreach (var xi in xs)
            {
                yield return interp1(x, y, xi, fixedDimension, fixedIndex);
            }
        }

        #endregion

        #region 2D interpolation
        /// <summary>
        /// Interpolation in two dimensions 
        /// </summary>
        /// <param name="f(x,y)">The known dependent values</param>
        /// <param name="x,y">The known independent values</param>
        /// <param name="xi,yi">Value to at which to interpolate</param>
        /// <returns>The interpolated value (clamped to boundary values if xi are of range)</returns>
        /// Assumptions: 1) x and y are monotonically increasing, and
        ///              2) xi and yi are inclusive to x and y respectively for interpolated results,
        ///              otherwise return NaN
        public static double interp2(IList<double> x, IList<double> y, double[,] f, double xi, double yi)
        {
            if ((x.Count != f.GetLength(0))||(y.Count != f.GetLength(1)))
            {
                throw new ArgumentException("Error in interp2: arrays x, y and f dimensions do not agree!");
            }
            int currentXIndex = 1;
            int currentYIndex = 1;

            //if ((xi < x[0]) || (yi < y[0]) || (xi > x[x.Count - 1]) || (yi > y[y.Count - 1])) return double.NaN;
            // changed this to clip to bounds (DC - 7/26/09)
            if ((xi <= x[0]) && (yi <= y[0])) return f[currentXIndex, currentYIndex];
            else if ((xi >= x[x.Count - 1]) && (yi >= y[y.Count - 1])) return f[x.Count - 1, y.Count - 1];
            else if (xi <= x[0]) return interp1(y, f, yi, 1, 0);
            else if (xi > x[x.Count - 1]) return interp1(y, f, yi, 1, x.Count - 1);
            else if (yi <= y[0]) return interp1(x, f, xi, 2, 0);
            else if (yi > y[y.Count - 1]) return interp1(x, f, xi, 2, y.Count - 1);
            else
            {
                // increment the index until you pass the desired interpolation point
                while (x[currentXIndex] < xi) currentXIndex++;
                while (y[currentYIndex] < yi) currentYIndex++;

                // then do the interp between x[currentXIndex-1] and xi[currentXIndex] and
                //                            y[currentYIndex-1] and yi[currentYIndex]
                double t = (xi - x[currentXIndex - 1]) / (x[currentXIndex] - x[currentXIndex - 1]);
                double u = (yi - y[currentYIndex - 1]) / (y[currentYIndex] - y[currentYIndex - 1]);
                return (1 - t) * (1 - u) * f[currentXIndex - 1, currentYIndex - 1] +
                             t * (1 - u) * f[currentXIndex, currentYIndex - 1] +
                                   t * u * f[currentXIndex, currentYIndex] +
                             (1 - t) * u * f[currentXIndex - 1, currentYIndex];
            }
        }

        #endregion
    }
}
