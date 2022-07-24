using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MPFitLib;
using Vts.Extensions;

namespace Vts.Modeling.Optimizers
{
    /// <summary>
    /// The <see cref="Optimizers"/> namespace contains the optimizer classes for the Virtual Tissue Simulator
    /// </summary>

    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }

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
        /// <summary>
        /// forward variables
        /// </summary>
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

            MPFit.Solve(MPFitFunc, data.Y.Length, pars.Length, a, pars, null, data, ref result);

            return a;
        }

        /// <summary>
        /// Optimization method.  This calls MPFit Levenberg Marquardt solver with constraints.
        /// </summary>
        /// <param name="a">optimization parameter initial guess</param>
        /// <param name="ia">accompanying array to <paramref name="a"/> that specifies which parameters to fit (held constant otherwise)</param>
        /// <param name="lowerBounds">accompanying array that specifies lower bounds for parameters</param>
        /// <param name="upperBounds">accompanying array that specifies upper bounds</param>
        /// <param name="y">"measured" values</param>
        /// <param name="ey">standard deviation values of <paramref name="y"/></param>
        /// <param name="forwardFunc">delegate function that evaluates the objective function given a parameter optimization array and (optional) constant variables</param>
        /// <param name="forwardVariables"></param>
        /// <returns>inverse solution array</returns>
        public double[] SolveWithConstraints(double[] a, bool[] ia, double[] lowerBounds, double[] upperBounds, double[] y, double[] ey, Func<double[], object[], double[]> forwardFunc, params object[] forwardVariables)
        {
            var data = new OptimizationData
            {
                Y = y,
                Ey = ey,
                ForwardFunc = forwardFunc,
                ForwardVariables = forwardVariables
            };

            mp_par[] pars = a.Select((ai, i) => new mp_par { isFixed = ia[i] ? 0 : 1 }).ToArray();
            for (int i = 0; i < pars.Length; i++)
            {
                pars[i].limited[0] = 1; // specify lower bound exists
                pars[i].limited[1] = 1; // specify upper bound exists
                pars[i].limits[0] = lowerBounds[i];
                pars[i].limits[1] = upperBounds[i];
            }

            mp_result result = new mp_result(a.Length);

            MPFit.Solve(MPFitFunc, data.Y.Length, pars.Length, a, pars, null, data, ref result);

            return a;
        }

        /// <summary>
        /// Standard function prototype that MPFit knows how to call. Use <paramref name="vars"/> to store information reqired to evaluate any objective function
        /// </summary>
        /// <param name="parameters">array of fit parameters</param>
        /// <param name="dy">array of residuals to be returned</param>
        /// <param name="dvec">not used</param>
        /// <param name="vars">private data</param>
        /// <returns>status return 0=successs, otherwise error</returns>
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

    }
}
