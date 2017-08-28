using System.Runtime.Serialization;

namespace Vts.MonteCarlo
{
    public class HenyeyGreensteinPhaseFunctionInput : IPhaseFunctionInput
    {
        public HenyeyGreensteinPhaseFunctionInput()
        {
            PhaseFunctionType = "HenyeyGreenstein";
        }

        /// <summary>
        /// Phase function type
        /// </summary>
        public string PhaseFunctionType { get; set; }
    }
}