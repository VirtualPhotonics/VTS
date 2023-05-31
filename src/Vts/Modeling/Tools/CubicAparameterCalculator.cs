namespace Vts.Modeling
{
    /// <summary>
    /// cubic polynomial calculator
    /// </summary>
    public static class CubicAparameterCalculator
    {
        /// <summary>
        /// Implementation of the cubic polynomial via Scott Prahl thesis 1988, Appendix A2.9
        /// </summary>
        /// <param name="n">Refractive Index</param>
        /// <returns>Returns the boundary parameter A</returns>
        public static double GetA(double n)
        {
            return -0.13755 * n * n * n + 4.3390 * n * n - 4.90466 * n + 1.68960;
        }

    }
}
