using System;
using System.Linq;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.VirtualBoundaries
{
    /// <summary>
    /// Implements IVirtualBoundary.  Used to capture surface radiance detectors
    /// </summary>
    public class RadianceVirtualBoundary : IVirtualBoundary
    {
        private readonly double _zPlanePosition;
        private readonly int _zDirection;

        /// <summary>
        /// Radiance virtual boundary
        /// </summary>
        /// <param name="detectorController">IDetectorController</param>
        /// <param name="name">string name</param>
        public RadianceVirtualBoundary(IDetectorController detectorController, string name)
        {
            DetectorController = detectorController;

            var detector = DetectorController.Detectors.FirstOrDefault(d => d.TallyDetails.IsInternalSurfaceTally);

            if (detector == null) return;
            _zPlanePosition = ((dynamic) detector).ZDepth;
            _zDirection = ((dynamic)detector).ZDirection;

            if (_zDirection > 0) // downward
            {
                WillHitBoundary = dp =>
                    dp.StateFlag.HasFlag(PhotonStateType.PseudoReflectedTissueBoundary) &&
                    dp.Direction.Uz > 0 &&
                    Math.Abs(dp.Position.Z - _zPlanePosition) < 10E-16;
            }
            // upward
            WillHitBoundary = dp =>
                dp.StateFlag.HasFlag(PhotonStateType.PseudoReflectedTissueBoundary) &&
                dp.Direction.Uz < 0 &&
                Math.Abs(dp.Position.Z - _zPlanePosition) < 10E-16;

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
        /// Finds the distance to the virtual boundary given direction of VB and photon.
        /// In all VBs that hold onto IHistory tallies this method does not calculate actual
        /// distance because that is done after trajectory is finished and entire history
        /// is processed.
        /// </summary>
        /// <param name="dp">photo data point</param>
        /// <returns>distance to virtual boundary</returns>
        public double GetDistanceToVirtualBoundary(PhotonDataPoint dp)
        {
            return double.PositiveInfinity;
        }

    }
}