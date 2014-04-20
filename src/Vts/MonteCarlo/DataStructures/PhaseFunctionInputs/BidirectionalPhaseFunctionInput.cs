using System.Runtime.Serialization;

namespace Vts.MonteCarlo
{
    public class BidirectionalPhaseFunctionInput : IPhaseFunctionInput
    {
        public BidirectionalPhaseFunctionInput()
        {
            PhaseFunctionType = PhaseFunctionType.Bidirectional;
        }

        /// <summary>
        /// Phase function type
        /// </summary>
        public PhaseFunctionType PhaseFunctionType { get; set; }
    }
}