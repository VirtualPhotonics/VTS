using System.Runtime.Serialization;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Bidirectional scattering function input
    /// </summary>
    public class BidirectionalPhaseFunctionInput : IPhaseFunctionInput
    {
        /// <summary>
        /// Bidirectional scattering function default constructor
        /// </summary>
        public BidirectionalPhaseFunctionInput()
        {
            PhaseFunctionType = "Bidirectional";
        }

        /// <summary>
        /// Phase function type
        /// </summary>
        public string PhaseFunctionType { get; set; }
    }
}