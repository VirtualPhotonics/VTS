using System.Runtime.Serialization;

namespace Vts.MonteCarlo.PhaseFunctionInputs
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
        [IgnoreDataMember]
        public PhaseFunctionType PhaseFunctionType { get; set; }
    }
}