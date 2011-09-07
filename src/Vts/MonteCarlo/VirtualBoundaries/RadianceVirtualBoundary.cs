using System;
using System.Linq;
using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Controllers;

namespace Vts.MonteCarlo.VirtualBoundaries
{
    /// <summary>
    /// Implements IVirtualBoundary.  Used to capture surface radiance detectors
    /// </summary>
    public class RadianceVirtualBoundary : IVirtualBoundary
    {
        //private ISurfaceDetectorController _detectorController;
        private IDetectorController _detectorController;
        private double _zPlanePosition;

        /// <summary>
        /// Creates an instance of a plane tranmission virtual boundary in direction given
        /// </summary>
        //public RadianceVirtualBoundary(ISurfaceDetectorController detectorController, string name)
        public RadianceVirtualBoundary(IDetectorController detectorController, string name)
        {
            _detectorController = detectorController;

            // not sure following is best design
            IDetector dosimetryDetector = DetectorController.Detectors.Where(d => d.TallyType == TallyType.RadianceOfRho).First();

            _zPlanePosition = ((RadianceOfRhoDetector)dosimetryDetector).ZDepth;

            WillHitBoundary = dp =>
                        dp.StateFlag.HasFlag(PhotonStateType.PseudoReflectedTissueBoundary) &&
                        dp.Direction.Uz > 0 &&
                        Math.Abs(dp.Position.Z - _zPlanePosition) < 10E-16;

            VirtualBoundaryType = VirtualBoundaryType.SurfaceRadiance;
            PhotonStateType = PhotonStateType.PseudoSurfaceRadianceVirtualBoundary;

            Name = name;
        }       

        ///// <summary>
        ///// Creates a default instance of a RadianceVB based on a plane at z=0, 
        ///// exiting tissue (in direction of z decreasing)
        ///// </summary>
        //public RadianceVirtualBoundary() 
        //    : this(null, null, null)
        //{
        //}

        public VirtualBoundaryType VirtualBoundaryType { get; private set; }
        public PhotonStateType PhotonStateType { get; private set; }
        public string Name { get; private set; }
        public Predicate<PhotonDataPoint> WillHitBoundary { get; private set; }
        public IDetectorController DetectorController { get { return _detectorController; } }

        /// <summary>
        /// Finds the distance to the virtual boundary given direction of VB and photon
        /// </summary>
        /// <param name="photon"></param>
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