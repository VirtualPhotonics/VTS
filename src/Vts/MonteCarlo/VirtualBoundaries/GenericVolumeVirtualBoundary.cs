using System;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.VirtualBoundaries
{
    /// <summary>
    /// Implements IVirtualBoundary.  Used to capture all internal volume detectors
    /// </summary>
    public class GenericVolumeVirtualBoundary : IVirtualBoundary
    {
        /// <summary>
        /// generic volume virtual boundary, used to capture all internal volume detectors e.g. FluenceOfRhoAndZ
        /// </summary>
        /// <param name="tissue">ITissue</param>
        /// <param name="detectorController">IDetectorController</param>
        /// <param name="name">string name</param>
        public GenericVolumeVirtualBoundary(ITissue tissue, IDetectorController detectorController, string name)
        {
            VirtualBoundaryType = VirtualBoundaryType.GenericVolumeBoundary;
            PhotonStateType = PhotonStateType.PseudoGenericVolumeVirtualBoundary;

            DetectorController = detectorController;

            Name = name;
        }

        /// <summary>
        /// VB type identifier
        /// </summary>
        public VirtualBoundaryType VirtualBoundaryType { get; }
        /// <summary>
        /// photon state type
        /// </summary>
        public PhotonStateType PhotonStateType { get; }
        /// <summary>
        /// string name
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// predicate of PhotonDataPoint determining whether will hit VB
        /// </summary>
        public Predicate<PhotonDataPoint> WillHitBoundary { get; }
        /// <summary>
        /// IDetectorController
        /// </summary>
        public IDetectorController DetectorController { get; }

        /// <summary>
        /// finds distance to VB
        /// </summary>
        /// <param name="dp">PhotonDataPoint</param>
        /// <returns>distance to virtual boundary</returns>
        public double GetDistanceToVirtualBoundary(PhotonDataPoint dp)
        {
            // not sure of following, could be zero
            return double.PositiveInfinity;
        }

    }
}
