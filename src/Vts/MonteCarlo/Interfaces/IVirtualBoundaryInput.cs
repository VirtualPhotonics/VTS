using System.Collections.Generic;
using Vts.MonteCarlo.Interfaces;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines contract for VirtualBoundaryInput used within the input to
    /// the Monte Carlo simulation, SimulationInput.
    /// </summary>
    public interface IVirtualBoundaryInput
    {
        /// <summary>
        /// List of IDetectorInput associated with this virtual boundary (VB)
        /// </summary>
        IList<IDetectorInput> DetectorInputs { get; }
        /// <summary>
        /// Flag indicating whether to write the results of this VB to database
        /// </summary>
        bool WriteToDatabase { get; }
        /// <summary>
        /// VirtualBoundaryType enum describing the type of this VB
        /// </summary>
        VirtualBoundaryType VirtualBoundaryType { get; }
        /// <summary>
        /// Name string identifying this VB
        /// </summary>
        string Name { get; }
    }
}