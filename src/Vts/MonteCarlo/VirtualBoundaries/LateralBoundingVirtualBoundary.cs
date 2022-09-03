using System;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.VirtualBoundaries
{
    /// <summary>
    /// Implements IVirtualBoundary.  Used to capture all photons absorbed by bounding 
    /// tissue region (whatever is dependency injected)
    /// </summary>
    public class LateralBoundingVirtualBoundary : IVirtualBoundary
    {
        private IDetectorController _detectorController;
        private ITissueRegion _boundingTissueRegion;


        /// <summary>
        /// diffuse reflectance VB
        /// </summary>
        /// <param name="tissue">ITissue</param>
        /// <param name="detectorController">IDetectorController</param>
        /// <param name="name">string name</param>
        public LateralBoundingVirtualBoundary(ITissue tissue, IDetectorController detectorController, string name)
        {
            _boundingTissueRegion = tissue.Regions[tissue.Regions.Count - 1]; // bounding region always last by convention

            WillHitBoundary = dp =>
                dp.StateFlag.HasFlag(PhotonStateType.PseudoBoundingVolumeTissueBoundary) &&
                _boundingTissueRegion.ContainsPosition(dp.Position);

            VirtualBoundaryType = VirtualBoundaryType.BoundingVolume;
            PhotonStateType = PhotonStateType.PseudoLateralBoundingVirtualBoundary;

            _detectorController = detectorController;

            Name = name;
        }

        /// <summary>
        /// VirtualBoundaryType enum indicating type of VB
        /// </summary>
        public VirtualBoundaryType VirtualBoundaryType { get; private set; }
        /// <summary>
        /// PhotonStateType enum indicating state of photon at this VB
        /// </summary>
        public PhotonStateType PhotonStateType { get; private set; }
        /// <summary>
        /// Name string of VB
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Predicate method to indicate if photon will hit VB boundary
        /// </summary>
        public Predicate<PhotonDataPoint> WillHitBoundary { get; private set; }
        /// <summary>
        /// IDetectorController specifying type of detector controller.
        /// </summary>
        public IDetectorController DetectorController { get { return _detectorController; } }

        /// <summary>
        /// finds distance to VB
        /// </summary>
        /// <param name="dp">PhotonDataPoint</param>
        /// <returns>distance to VB</returns>
        public double GetDistanceToVirtualBoundary(PhotonDataPoint dp)
        {
            double distanceToBoundary = double.PositiveInfinity;
            // check if VB not applied
            if (!dp.StateFlag.HasFlag(PhotonStateType.PseudoBoundingVolumeTissueBoundary))
            {
                return distanceToBoundary;
            }
            // VB applies
            distanceToBoundary = 0.0; // if gotten here sitting on bounding volume
            return distanceToBoundary;
        }
    }
}
