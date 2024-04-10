using System;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.VirtualBoundaries
{
    /// <summary>
    /// Implements IVirtualBoundary.  Used to capture Outer cylinder reflectance detectors
    /// </summary>
    public class InfiniteCylinderSurfaceDotNormalNegativeVirtualBoundary : IVirtualBoundary
    {
        private readonly double distanceToBoundary;

        /// <summary>
        /// Outer cylinder reflectance VB
        /// </summary>
        /// <param name="tissue">ITissue</param>
        /// <param name="detectorController">IDetectorController</param>
        /// <param name="name">string name</param>
        public InfiniteCylinderSurfaceDotNormalNegativeVirtualBoundary(ITissue tissue, IDetectorController detectorController, string name)
        {
            // 1. Use "ContainsPosition(Position position)" to check whether photon is insde the outer cylinder or not
            // 2. If no, WillHitBoundary = false
            // 3. If yes, use "RayIntersectBoundary(Photon photon, out double distanceToBoundary)" to compute distanceToBoundary

            VirtualBoundaryType = VirtualBoundaryType.InfiniteCylinderSurfaceDotNormalPositive;
            //PhotonStateType = PhotonStateType.PseudoInfiniteCylinderSurfaceDotNormalPositiveVirtualBoundary;

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
            // check if VB not applied
            //if (!dp.StateFlag.HasFlag(PhotonStateType.PseudoInfiniteCylinderSurfaceDotNormalPositiveVirtualBoundary) ||
            //    dp.Direction.Uz >= 0.0)
            //{
            //    return double.PositiveInfinity;
            //}
            // VB applies            
            return distanceToBoundary;
        }
    }
}
