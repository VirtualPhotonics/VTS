using System;

namespace Vts
{
    /// <summary>
    /// This is the contract for the inverse solver
    /// </summary>
    public interface IOldOptimizer
    {
        /// <summary>
        /// This is a general solver routine using mrqmin
        /// </summary>
        /// <param name="a">An array of the input parameters (ie. optical properties)</param>
        /// <param name="ia">An array specifies which input parameters to fit (others will be fixed)</param>
        /// <param name="xdata">The independent variable (ie. rho)</param>
        /// <param name="ydata">The dependent variable (ie. measured diffuse reflectance)</param>
        /// <param name="ystdev">The standard deviation of ydata</param>
        /// <returns></returns>
        double[] Solve(double[] a, bool[] ia, double[] xdata, double[] ydata, double[] ystdev, 
            Func<double, double[], double[]> func);
    }
}
