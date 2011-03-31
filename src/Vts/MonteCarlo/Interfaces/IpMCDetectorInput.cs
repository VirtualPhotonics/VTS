using System.Collections.Generic;

namespace Vts.MonteCarlo.Interfaces
{
    public interface IpMCDetectorInput : IDetectorInput
    {
        IList<OpticalProperties> PerturbedOps { get; set; }
        IList<int> PerturbedRegionsIndices { get; set; }
    }
}
