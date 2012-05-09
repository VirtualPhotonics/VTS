using System.Collections.Generic;

namespace Vts.MonteCarlo
{
    public interface IPhaseFunctionInput
    {
        /// <summary>
        /// PhaseFunctionType enum identifying type of phase function
        /// </summary>
        PhaseFunctionType PhaseFunctionType { get; set; }
    }
}