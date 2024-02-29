using System;
using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.VirtualBoundaries
{
    /// <summary>
    /// Implements IVirtualBoundary.  Used to capture Inner cylinder reflectance detectors
    /// </summary>
    public class InnerCylinderReflectanceVirtualBoundary : IVirtualBoundary
    {
        private readonly double distanceToBoundary;

        /// <summary>
        /// Inner cylinder reflectance VB
        /// </summary>
        /// <param name="tissue">ITissue</param>
        /// <param name="detectorController">IDetectorController</param>
        /// <param name="name">string name</param>
        public InnerCylinderReflectanceVirtualBoundary(ITissue tissue, IDetectorController detectorController, string name)
        {
            // 1. Use "ContainsPosition(Position position)" to check whether photon is insde the inner cylinder or not
            // 2. If yes, WillHitBoundary = false
            // 3. If no, compute dot product of "SurfaceNormal" and "photon direction"
            //     a. if dot product is positive (>=0), WillHitBoundary = false
            //     b. if dotproduct is negative (<0), apply "RayIntersectBoundary(Photon photon, out double distanceToBoundary)" to compute distanceToBoundary

            VirtualBoundaryType = VirtualBoundaryType.InnerCylinderReflectance;
            PhotonStateType = PhotonStateType.PseudoInnerCylinderVirtualBoundary;

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
            if (!dp.StateFlag.HasFlag(PhotonStateType.PseudoInnerCylinderVirtualBoundary))
            {
                return double.PositiveInfinity;
            }
            // VB applies            
            return distanceToBoundary;
        }
    }
}
