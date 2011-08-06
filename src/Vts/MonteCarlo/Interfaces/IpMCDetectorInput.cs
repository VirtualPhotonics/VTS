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
        IList<OpticalProperties> PerturbedOps { get; set; }
        IList<int> PerturbedRegionsIndices { get; set; }
    }
}
