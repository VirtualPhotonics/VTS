using System.Collections.Generic;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// interface for phase function input
    /// </summary>
    public interface IPhaseFunctionInput
    {
        /// <summary>
        /// string identifying type of phase function
        /// </summary>
        string PhaseFunctionType { get; set; }
    }
}