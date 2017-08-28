using System.Runtime.Serialization;

namespace Vts.MonteCarlo
{
    public class BidirectionalPhaseFunctionInput : IPhaseFunctionInput
    {
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