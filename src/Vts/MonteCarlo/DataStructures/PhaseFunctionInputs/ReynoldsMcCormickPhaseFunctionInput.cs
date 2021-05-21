

namespace Vts.MonteCarlo
{
    public class ReynoldsMcCormickPhaseFunctionInput : IPhaseFunctionInput
    {
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