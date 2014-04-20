using System.Runtime.Serialization;

namespace Vts.MonteCarlo
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
        public PhaseFunctionType PhaseFunctionType { get; set; }
    }
}