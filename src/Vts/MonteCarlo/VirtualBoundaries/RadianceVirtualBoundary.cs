using System;
using System.Linq;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.VirtualBoundaries
{
    /// <summary>
    /// Implements IVirtualBoundary.  Used to capture surface radiance detectors
    /// </summary>
    public class RadianceVirtualBoundary : IVirtualBoundary
    {
        private IDetectorController _detectorController;
        private double _zPlanePosition;

        /// <summary>
        /// Radiance virtual boundary
        /// </summary>
        /// <param name="detectorController">IDetectorController</param>
        /// <param name="name">string name</param>
        public RadianceVirtualBoundary(IDetectorController detectorController, string name)
        {
            _detectorController = detectorController;

            // not sure following is best design
            IDetector dosimetryDetector = DetectorController.Detectors.Where(d => d.TallyType == TallyType.RadianceOfRho).FirstOrDefault();

            if (dosimetryDetector != null)
            {
                _zPlanePosition = ((RadianceOfRhoDetector) dosimetryDetector).ZDepth;

                WillHitBoundary = dp =>
                                  dp.StateFlag.HasFlag(PhotonStateType.PseudoReflectedTissueBoundary) &&
                                  dp.Direction.Uz > 0 &&
                                  Math.Abs(dp.Position.Z - _zPlanePosition) < 10E-16;

                VirtualBoundaryType = VirtualBoundaryType.SurfaceRadiance;
                PhotonStateType = PhotonStateType.PseudoSurfaceRadianceVirtualBoundary;

                Name = name;
            }
        }       

        ///// <summary>
        ///// Creates a default instance of a RadianceVB based on a plane at z=0, 
        ///// exiting tissue (in direction of z decreasing)
        ///// </summary>
        //public RadianceVirtualBoundary() 
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
        /// Name
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// predicate of PhotonDataPoint providing whether photon will hit VB
        /// </summary>
        public Predicate<PhotonDataPoint> WillHitBoundary { get; private set; }
        /// <summary>
        /// IDetectorController
        /// </summary>
        public IDetectorController DetectorController { get { return _detectorController; } }

        /// <summary>
        /// Finds the distance to the virtual boundary given direction of VB and photon
        /// </summary>
        /// <param name="dp">photo data point</param>
        public double GetDistanceToVirtualBoundary(PhotonDataPoint dp)
        {
            double distanceToBoundary = double.PositiveInfinity;

            // check if VB not applied
            //if (!dp.StateFlag.Has(PhotonStateType.PseudoRadianceVirtualBoundary) ||
            //    dp.Direction.Uz <= 0.0)

            // since no tissue boundary here, need other checks for whether VB is applied
            if ((dp.Direction.Uz <= 0.0) || (dp.Position.Z >= _zPlanePosition)) // >= is key here
            {
                return distanceToBoundary;
            }
            // VB applies
            distanceToBoundary = (_zPlanePosition - dp.Position.Z) / dp.Direction.Uz;
          
            return distanceToBoundary;
        }

    }
}