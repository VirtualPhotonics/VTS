using System.Runtime.Serialization;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Henyey-Greenstein phase function input
    /// </summary>
    public class HenyeyGreensteinPhaseFunctionInput : IPhaseFunctionInput
    {
        /// <summary>
        /// Henyey-Greenstein phase function
        /// </summary>
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