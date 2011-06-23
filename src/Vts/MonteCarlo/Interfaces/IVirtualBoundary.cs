using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for Virtual Boundary classes in Monte Carlo simulation.
    /// </summary>
    public interface IVirtualBoundary
    {
        IDetectorController DetectorController { get; set; } // need set for VBFactory
        double GetDistanceToVirtualBoundary(Photon photon);
        string Name { get; set; }
        VirtualBoundaryType VirtualBoundaryType { get; set; }
    }
}
