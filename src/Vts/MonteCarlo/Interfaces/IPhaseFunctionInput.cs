using System.Collections.Generic;

namespace Vts.MonteCarlo
{
    public interface IPhaseFunctionInput
    {
        /// <summary>
        /// string identifying type of phase function
        /// </summary>
        string PhaseFunctionType { get; set; }
    }
}