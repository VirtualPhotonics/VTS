using System;
using System.Linq;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.VirtualBoundaries
{
    /// <summary>
    /// Implements IVirtualBoundary.  Used to capture surface radiance detectors
    /// </summary>
    public class RadianceVirtualBoundary : IVirtualBoundary
    {
        private readonly double _zPlanePosition;

        /// <summary>
        /// Radiance virtual boundary
        /// </summary>
        /// <param name="detectorController">IDetectorController</param>
        /// <param name="name">string name</param>
        public RadianceVirtualBoundary(IDetectorController detectorController, string name)
        {
            DetectorController = detectorController;

            var dosimetryDetector = DetectorController.Detectors.FirstOrDefault(d => d.TallyDetails.IsInternalSurfaceTally);

            if (dosimetryDetector != null)
            {
                _zPlanePosition = ((dynamic) dosimetryDetector).ZDepth;

                WillHitBoundary = dp =>
                                  dp.StateFlag.HasFlag(PhotonStateType.PseudoReflectedTissueBoundary) &&
                                  dp.Direction.Uz > 0 &&
                                  Math.Abs(dp.Position.Z - _zPlanePosition) < 10E-16;

                VirtualBoundaryType = VirtualBoundaryType.InternalSurface;
                PhotonStateType = PhotonStateType.PseudoSurfaceRadianceVirtualBoundary;

                Name = name;
            }
        }       

        /// <summary>
        /// VirtualBoundaryType
        /// </summary>
        public VirtualBoundaryType VirtualBoundaryType { get; }
        /// <summary>
        /// PhotonStateType
        /// </summary>
        public PhotonStateType PhotonStateType { get; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// predicate of PhotonDataPoint providing whether photon will hit VB
        /// </summary>
        public Predicate<PhotonDataPoint> WillHitBoundary { get; }
        /// <summary>
        /// IDetectorController
        /// </summary>
        public IDetectorController DetectorController { get; }

        /// <summary>
        /// Finds the distance to the virtual boundary given direction of VB and photon
        /// </summary>
        /// <param name="dp">photo data point</param>
        /// <returns>distance to virtual boundary</returns>
        public double GetDistanceToVirtualBoundary(PhotonDataPoint dp)
        {
            var distanceToBoundary = double.PositiveInfinity;

            // since no tissue boundary here, need other checks for whether VB is applied
            if ((dp.Direction.Uz <= 0.0) || (dp.Position.Z >= _zPlanePosition)) // >= is key here
            {
                return distanceToBoundary; // return infinity
            }
            // VB applies
            distanceToBoundary = (_zPlanePosition - dp.Position.Z) / dp.Direction.Uz;
          
            return distanceToBoundary;
        }

    }
}