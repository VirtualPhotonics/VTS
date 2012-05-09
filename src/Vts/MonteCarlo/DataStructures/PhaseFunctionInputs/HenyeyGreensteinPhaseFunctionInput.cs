using System.Runtime.Serialization;

namespace Vts.MonteCarlo.PhaseFunctionInputs
{
    public class HenyeyGreensteinPhaseFunctionInput : IPhaseFunctionInput
    {
        public HenyeyGreensteinPhaseFunctionInput()
        {
            PhaseFunctionType = PhaseFunctionType.HenyeyGreenstein;
        }

        /// <summary>
        /// Phase function type
        /// </summary>
        [IgnoreDataMember]
        public PhaseFunctionType PhaseFunctionType { get; set; }
    }
}