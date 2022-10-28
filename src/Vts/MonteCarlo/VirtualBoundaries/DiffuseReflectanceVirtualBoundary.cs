using System;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.VirtualBoundaries
{
    /// <summary>
    /// Implements IVirtualBoundary.  Used to capture all diffuse reflectance detectors
    /// </summary>
    public class DiffuseReflectanceVirtualBoundary : IVirtualBoundary
    {
        private readonly double _zPlanePosition;

        /// <summary>
        /// diffuse reflectance VB
        /// </summary>
        /// <param name="tissue">ITissue</param>
        /// <param name="detectorController">IDetectorController</param>
        /// <param name="name">string name</param>
        public DiffuseReflectanceVirtualBoundary(ITissue tissue, IDetectorController detectorController, string name)
        {
            _zPlanePosition = ((LayerTissueRegion)tissue.Regions[0]).ZRange.Stop;

            WillHitBoundary = dp =>
                        dp.StateFlag.HasFlag(PhotonStateType.PseudoReflectedTissueBoundary) &&
                        dp.Direction.Uz < 0 &&
                        Math.Abs(dp.Position.Z - _zPlanePosition) < 10E-16;

            VirtualBoundaryType = VirtualBoundaryType.DiffuseReflectance;
            PhotonStateType = PhotonStateType.PseudoDiffuseReflectanceVirtualBoundary;

            DetectorController = detectorController;

            Name = name;
        }

        /// <summary>
        /// VirtualBoundaryType enum indicating type of VB
        /// </summary>
        public VirtualBoundaryType VirtualBoundaryType { get; }
        /// <summary>
        /// PhotonStateType enum indicating state of photon at this VB
        /// </summary>
        public PhotonStateType PhotonStateType { get; }
        /// <summary>
        /// Name string of VB
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Predicate method to indicate if photon will hit VB boundary
        /// </summary>
        public Predicate<PhotonDataPoint> WillHitBoundary { get; }
        /// <summary>
        /// IDetectorController specifying type of detector controller.
        /// </summary>
        public IDetectorController DetectorController { get; }

        /// <summary>
        /// finds distance to VB
        /// </summary>
        /// <param name="dp">PhotonDataPoint</param>
        /// <returns>distance to VB</returns>
        public double GetDistanceToVirtualBoundary(PhotonDataPoint dp)
        {
            var distanceToBoundary = double.PositiveInfinity;
            // check if VB not applied
            if (!dp.StateFlag.HasFlag(PhotonStateType.PseudoReflectedTissueBoundary) ||
                dp.Direction.Uz >= 0.0)
            {
                return distanceToBoundary;
            }
            // VB applies
            distanceToBoundary = (_zPlanePosition - dp.Position.Z) / dp.Direction.Uz;
            return distanceToBoundary;
        }
    }
}
