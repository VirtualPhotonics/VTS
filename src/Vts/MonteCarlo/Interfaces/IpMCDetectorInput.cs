using System.Collections.Generic;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This is a contract for perturbation Monte Carlo (pMC) detector inputs.
    /// It describes the additional properties from IDetectorInput needed to
    /// perform the pMC detection.
    /// </summary>
    public interface IpMCDetectorInput : IDetectorInput
    {
        /// <summary>
        /// list of perturbed optical properties.  list indexing follows indexing of tissue.
        /// </summary>
        IList<OpticalProperties> PerturbedOps { get; set; }
        /// <summary>
        /// list of perturbed region indices.  list indexing follows indexing of tissue.
        /// </summary>
        IList<int> PerturbedRegionsIndices { get; set; }
    }
}
