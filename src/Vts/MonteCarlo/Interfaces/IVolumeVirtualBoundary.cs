using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using System;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for Virtual Boundary classes in Monte Carlo simulation.
    /// </summary>
    public interface IVolumeVirtualBoundary : IVirtualBoundary
    {
        IVolumeDetectorController DetectorController { get; }
    }
}
