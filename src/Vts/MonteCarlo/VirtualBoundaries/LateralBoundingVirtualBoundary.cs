using System;
using System.Runtime.CompilerServices;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.VirtualBoundaries
{
    /// <summary>
    /// The <see cref="VirtualBoundaries"/> namespace contains the Monte Carlo virtual boundaries to which detectors attach
    /// </summary>

    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }

    /// <summary>
    /// Implements IVirtualBoundary.  Used to capture all photons absorbed by bounding 
    /// tissue region (whatever is dependency injected)
    /// </summary>
    public class LateralBoundingVirtualBoundary : IVirtualBoundary
    {
        /// <summary>
        /// diffuse lateral boundary VB
        /// </summary>
        /// <param name="tissue">ITissue</param>
        /// <param name="detectorController">IDetectorController</param>
        /// <param name="name">string name</param>
        public LateralBoundingVirtualBoundary(ITissue tissue, IDetectorController detectorController, string name)
        {
            var boundingTissueRegion = tissue.Regions[tissue.Regions.Count - 1]; // bounding region always last by convention

            WillHitBoundary = dp =>
                dp.StateFlag.HasFlag(PhotonStateType.PseudoBoundingVolumeTissueBoundary) &&
                boundingTissueRegion.ContainsPosition(dp.Position);

            VirtualBoundaryType = VirtualBoundaryType.BoundingVolume;
            PhotonStateType = PhotonStateType.PseudoLateralBoundingVirtualBoundary;

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
