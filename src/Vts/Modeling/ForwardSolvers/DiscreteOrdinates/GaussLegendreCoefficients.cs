
namespace Vts.Modeling.ForwardSolvers.DiscreteOrdinates
{
    /// <summary>
    /// Gauss Legendre coefficients
    /// </summary>
    public struct GaussLegendreCoefficients
    {
        /// <summary>
        /// cosine theta?
        /// </summary>
        public double[] mu { get; set; }

        /// <summary>
        /// weights
        /// </summary>
        public double[] wt { get; set; }
    }
}
