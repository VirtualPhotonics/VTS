using System;

namespace Vts
{
    /// <summary>
    /// This is the contract for the inverse solver
    /// </summary>
    public interface IOptimizer
    {
        /// <summary>
        /// Standard interface for all optimization libraries
        /// </summary>
        /// <param name="a">optimization parameter initial guess</param>
        /// <param name="ia">accompanying array to <paramref name="a"/> that specifies which parameters to fit (held constant otherwise)</param>
        /// <param name="y">"measured" values</param>
        /// <param name="ey">standard deviation values of <paramref name="y"/></param>
        /// <param name="forwardFunc">delegate function that evaluates the objective function given a parameter optimization array and (optional) constant variables</param>
        /// <param name="forwardVariables"></param>
        double[] Solve(double[] a, bool[] ia, double[] y, double[] ey, 
            Func<double[], object[], double[]> forwardFunc,  params object[] forwardVariables);

    }
}