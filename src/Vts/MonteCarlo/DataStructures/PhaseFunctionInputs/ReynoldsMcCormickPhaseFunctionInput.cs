

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Reynolds-McCormick scattering phase function
    /// </summary>
    public class ReynoldsMcCormickPhaseFunctionInput : IPhaseFunctionInput
    {
        /// <summary>
        /// Reynolds-McCormick scattering phase function input
        /// </summary>
        /// <param name="alpha">R-M alpha parameter</param>
        public ReynoldsMcCormickPhaseFunctionInput(double alpha)
        {
            PhaseFunctionType = "ReynoldsMcCormick";
            Alpha = alpha;
        }
        /// <summary>
        /// default constructor
        /// </summary>
        public ReynoldsMcCormickPhaseFunctionInput() : this(0.5) { }

        /// <summary>
        /// Phase function type
        /// </summary>
        public string PhaseFunctionType { get; set; }
        /// <summary>
        /// Phase function second parameter
        /// </summary>
        public double Alpha { get; set; }
    }
}