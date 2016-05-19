using System;
using System.Collections.Generic;
using System.Linq;
using MPFitLib;
using Vts.Extensions;

namespace Vts.Modeling.Optimizers
{
    /// <summary>
    /// Data input to the optimization method Solve.
    /// ForwardFunc:delegate function that evaluates the objective function given a parameter optimization array and (optional) constant variables
    /// Y: measured data
    /// Ey: standard deviation of Y
    /// </summary>
    public class OptimizationData
    {
        /// <summary>
        /// delegate function that evaluates the objective function given a parameter optimization array and (optional) constant variables
        /// </summary>
        public Func<double[], object[], double[]> ForwardFunc { get; set; }
        public object[] ForwardVariables { get; set; }
        /// <summary>
        /// measured data
        /// </summary>
        public double[] Y { get; set; }
        /// <summary>
        /// standard deviation of Y
        /// </summary>
        public double[] Ey { get; set; }
    }
    /// <summary>
    /// MPFit Levenberg Marquardt least-squares minimization.
    /// </summary>
    public class MPFitLevenbergMarquardtOptimizer : IOptimizer
    {
        /// <summary>
        /// Optimization method.  This calls MPFit Levenberg Marquardt solver.
        /// For examples of usage, see TestMPFit.cs:
        /// https://csmpfit.codeplex.com/SourceControl/latest#src/MPFitLib.Test/TestMPFit.cs
        /// For an example of a call with the objective function:
        /// https://csmpfit.codeplex.com/SourceControl/latest#src/MPFitLib.Test/ForwardModels.cs
        /// </summary>
        /// <param name="a">optimization parameter initial guess</param>
        /// <param name="ia">accompanying array to <paramref name="a"/> that specifies which parameters to fit (held constant otherwise)</param>
        /// <param name="y">"measured" values</param>
        /// <param name="ey">standard deviation values of <paramref name="y"/></param>
        /// <param name="forwardFunc">delegate function that evaluates the objective function given a parameter optimization array and (optional) constant variables</param>
        /// <param name="forwardVariables"></param>
        public double[] Solve(double[] a, bool[] ia, double[] y, double[] ey, Func<double[], object[], double[]> forwardFunc, params object[] forwardVariables)
        {
            var data = new OptimizationData
            {
                Y = y, 
                Ey = ey, 
                ForwardFunc = forwardFunc,
                ForwardVariables = forwardVariables
            };

            mp_par[] pars = a.Select((ai, i) => new mp_par { isFixed = ia[i] ? 0 : 1 }).ToArray();

            mp_result result = new mp_result(a.Length);

            int status = MPFit.Solve(MPFitFunc, data.Y.Length, pars.Length, a, pars, null, data, ref result);

            return a;
        }

        /// <summary>
        /// Standard function prototype that MPFit knows how to call. Use <paramref name="vars"/> to store information reqired to evaluate any objective function
        /// </summary>
        /// <param name="parameters">array of fit parameters</param>
        /// <param name="dy">array of residuals to be returned</param>
        /// <param name="dvec">not used</param>
        /// <param name="vars">private data</param>
        /// <returns></returns>
        private static int MPFitFunc(double[] parameters, double[] dy, IList<double>[] dvec, object vars)
        {
            OptimizationData oData = vars as OptimizationData;
            if (oData != null)
            {
                var differenceValues = oData
                    .ForwardFunc(parameters, oData.ForwardVariables)
                    .Zip(oData.Y, oData.Ey, (fi, yi, eyi) => eyi != 0 ? (yi - fi)/eyi : 0.0);

                dy.PopulateFromEnumerable(differenceValues);

                return 0; // success
            }

            return MPFit.MP_ERR_PARAM;
        }

        //{

        ///// <summary>
        ///// User-function delegate structure required by MPFit.Solve
        ///// </summary>
        ///// <param name="m">Number of functions (elemens of fvec)</param>
        ///// <param name="npar">Number of variables (elements of x)</param>
        ///// <param name="x">I - Parameters</param>
        ///// <param name="fvec">O - function values</param>
        ///// <param name="dvec">
        ///// O - function derivatives (optional)
        ///// "Array of ILists" to accomodate DelimitedArray IList implementation
        ///// </param>
        ///// <param name="prv">I/O - function private data (cast to object type in user function)</param>
        ///// <returns></returns>
        //public delegate int mp_func(int m, int npar, double[] x, double[] fvec, IList<double>[] dvec, object prv);

        /* 
         * linear fit function
         *
         * m - number of data points
         * n - number of parameters (2)
         * p - array of fit parameters 
         * dy - array of residuals to be returned
         * CustomUserVariable - private data (struct vars_struct *)
         *
         * RETURNS: error code (0 = success)
         */
        //public static int LinFunc(int m, int n, double[] p, double[] dy, IList<double>[] dvec, object vars)
        //{
        //    int i;
        //    double[] x, y, ey;
        //    double f;

        //    CustomUserVariable v = (CustomUserVariable)vars;

        //    x = v.X;
        //    y = v.Y;
        //    ey = v.Ey;

        //    for (i = 0; i < m; i++)
        //    {
        //        f = p[0] - p[1] * x[i];     /* Linear fit function */
        //        dy[i] = (y[i] - f) / ey[i];
        //    }

        //    return 0;
        //}
    }
}
