using System;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for Virtual Boundary classes in Monte Carlo simulation.
    /// </summary>
    public interface IVirtualBoundary
    {
        /// <summary>
        /// Predicate method to determine if photon will hit virtual boundary (VB)
        /// </summary>
        Predicate<PhotonDataPoint> WillHitBoundary { get; }
        /// <summary>
        /// Method to determine distance to VB
        /// </summary>
        /// <param name="dp">PhotonDataPoint current location</param>
        /// <returns>distance to VB</returns>
        double GetDistanceToVirtualBoundary(PhotonDataPoint dp);
        /// <summary>
        /// Name of VB, default VirtualBoundaryType.ToString()
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Type of the VB
        /// </summary>
        VirtualBoundaryType VirtualBoundaryType { get; }
        /// <summary>
        /// State of the photon at this VB
        /// </summary>
        PhotonStateType PhotonStateType { get; }
        /// <summary>
        /// Each VB holds on to a detector controller which manages the detectors associated with this VB 
        /// </summary>
        IDetectorController DetectorController { get; }
    }
}
