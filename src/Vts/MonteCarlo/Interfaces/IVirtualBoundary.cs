using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using System;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for Virtual Boundary classes in Monte Carlo simulation.
    /// </summary>
    public interface IVirtualBoundary
    {
        Predicate<PhotonDataPoint> WillHitBoundary { get; }
        double GetDistanceToVirtualBoundary(PhotonDataPoint dp);
        string Name { get; }
        VirtualBoundaryType VirtualBoundaryType { get; }
        PhotonStateType PhotonStateType { get; }
    }
}
