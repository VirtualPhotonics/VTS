using System;
using System.Linq;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.VirtualBoundaries
{
    /// <summary>
    /// Implements IVirtualBoundary.  Used to capture internal surface radiance detectors.
    /// These surfaces could be a z=constant plane (for dosimetry) in downward direction, or
    /// cylinder surface in outward or inward direction depending on detector AXIS definition
    /// </summary>
    public class RadianceVirtualBoundary : IVirtualBoundary
    {
        private readonly double _zPlanePosition = -1; // set to something not possible
        private readonly ITissueRegion _infiniteCylinder;
        private readonly int _cylinderRegionTissueIndex;
        private readonly ITissue _tissue;

        /// <summary>
        /// Radiance virtual boundary
        /// </summary>
        /// <param name="tissue">tissue definition</param>
        /// <param name="detectorController">IDetectorController</param>
        /// <param name="name">string name</param>
        public RadianceVirtualBoundary(ITissue tissue, IDetectorController detectorController, string name)
        {
            DetectorController = detectorController;
            _tissue = tissue;

            var internalSurfaceDetector = DetectorController.Detectors.FirstOrDefault(d => d.TallyDetails.IsInternalSurfaceTally);

            if (internalSurfaceDetector == null) return;

            // check which type of detector(s) attached to this VB
            if (internalSurfaceDetector.TallyType == TallyType.RadianceOfRhoAtZ)
            {
                _zPlanePosition = ((RadianceOfRhoAtZDetector)internalSurfaceDetector).ZDepth;

                WillHitBoundary = dp =>
                    dp.Direction.Uz > 0 &&
                    Math.Abs(dp.Position.Z - _zPlanePosition) < 10E-16;
            }

            if (internalSurfaceDetector.TallyType == TallyType.InternalSurfaceFiber)
            {
                _cylinderRegionTissueIndex = ((InternalSurfaceFiberDetector)internalSurfaceDetector)
                    .FinalTissueRegionIndex;
                _infiniteCylinder = tissue.Regions[_cylinderRegionTissueIndex];
               
                WillHitBoundary = dp =>
                    _infiniteCylinder.RayIntersectBoundary(
                        new Photon(dp.Position, dp.Direction, dp.Weight, tissue, _cylinderRegionTissueIndex, null),
                        out var distanceToBoundary);
            }

            VirtualBoundaryType = VirtualBoundaryType.InternalSurface;
            PhotonStateType = PhotonStateType.PseudoSurfaceRadianceVirtualBoundary;

            Name = name;
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

            // determine which surface
            if (_zPlanePosition >= 0)
            {
                // since no tissue boundary here, need other checks for whether VB is applied
                if (dp.Direction.Uz <= 0.0 || dp.Position.Z >= _zPlanePosition) // >= is key here
                {
                    return distanceToBoundary; // return infinity
                }

                // VB applies
                distanceToBoundary = (_zPlanePosition - dp.Position.Z) / dp.Direction.Uz;
            }
            else // infinite cylinder
            {
                // not sure if I should be calling tissue region methods here
                var photon = new Photon(
                    dp.Position, 
                    dp.Direction, 
                    dp.Weight, 
                    _tissue, 
                    _cylinderRegionTissueIndex,
                    null) // no need for RNG here
                {
                    S = 100
                }; 
                _infiniteCylinder.RayIntersectBoundary(photon, out distanceToBoundary);
            }

            return distanceToBoundary;
        }

    }
}