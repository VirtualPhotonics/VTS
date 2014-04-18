using System;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.VirtualBoundaries
{
    /// <summary>
    /// Implements IVirtualBoundary.  Used to capture all diffuse reflectance detectors
    /// </summary>
    public class pMCDiffuseReflectanceVirtualBoundary : IVirtualBoundary
    {
        private IDetectorController _detectorController;
        private double _zPlanePosition;

        /// <summary>
        /// class for perturbation Monte Carlo (pMC) diffuse reflectance virtual boundary
        /// </summary>
        /// <param name="tissue">ITissue</param>
        /// <param name="detectorController">IDetectorController</param>
        /// <param name="name">string name</param>
        public pMCDiffuseReflectanceVirtualBoundary(ITissue tissue, IDetectorController detectorController, string name)
        {
            _zPlanePosition = ((LayerRegion)tissue.Regions[0]).ZRange.Stop;

            WillHitBoundary = dp =>
                        dp.StateFlag.HasFlag(PhotonStateType.PseudoReflectedTissueBoundary) &&
                        dp.Direction.Uz < 0 &&
                        Math.Abs(dp.Position.Z - _zPlanePosition) < 10E-16;

            VirtualBoundaryType = VirtualBoundaryType.pMCDiffuseReflectance;
            PhotonStateType = PhotonStateType.PseudoDiffuseReflectanceVirtualBoundary;

            _detectorController = detectorController;

            Name = name;
        }       

        ///// <summary>
        ///// Creates a default instance of a PlanarTransmissionVB based on a plane at z=0, 
        ///// exiting tissue (in direction of z decreasing)
        ///// </summary>
        //public DiffuseReflectanceVirtualBoundary() 
        //    : this(null, null, null)
        //{
        //}
        /// <summary>
        /// VirtualBoundaryType
        /// </summary>
        public VirtualBoundaryType VirtualBoundaryType { get; private set; }
        /// <summary>
        /// PhotonStateType
        /// </summary>
        public PhotonStateType PhotonStateType { get; private set; }
        /// <summary>
        /// string Name
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Predicate of PhotonDataPoint
        /// </summary>
        public Predicate<PhotonDataPoint> WillHitBoundary { get; private set; }
        /// <summary>
        /// IDetectorController
        /// </summary>
        public IDetectorController DetectorController { get { return _detectorController; } }

        /// <summary>
        /// Finds the distance to the virtual boundary given direction of VB and photon
        /// </summary>
        /// <param name="dp">photon data point</param>
        public double GetDistanceToVirtualBoundary(PhotonDataPoint dp)
        {
            double distanceToBoundary = double.PositiveInfinity;
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
