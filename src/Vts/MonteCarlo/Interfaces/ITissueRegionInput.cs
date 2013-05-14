using System.Collections.Generic;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for TissueRegionInput classes.
    /// </summary>
    public interface ITissueRegionInput
    {
        /// <summary>
        /// Optical Properties of this region
        /// </summary>
        OpticalProperties OP { get; set; }
        /// <summary>
        /// Phase function type of this region
        /// </summary>
        PhaseFunctionType PhaseFuncType { get; set; }
        /// <summary>
        /// key of the phasefunction in the phase function and phase function input dictionaries.
        /// </summary>
        string Key;
    }
}
