using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.Extensions;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Controllers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.VirtualBoundaries
{
    /// <summary>
    /// Implements IVirtualBoundary.  Used to capture all reflectance/transmittance detectors
    /// </summary>
    public class PlanarTransmissionVirtualBoundary : IVirtualBoundary
    {
        private VirtualBoundaryAxisType _axis;
        private double _planeValue;
        private VirtualBoundaryDirectionType _direction;
        private IDetectorController _detectorController;

        /// <summary>
        /// Creates an instance of a plane tranmission virtual boundary in direction given
        /// </summary>
        public PlanarTransmissionVirtualBoundary(
            Predicate<PhotonDataPoint> willHitBoundary,
            VirtualBoundaryAxisType axis,
            VirtualBoundaryDirectionType direction,
            double planeValue,
            VirtualBoundaryType type,
            string name)
        {
            WillHitBoundary = willHitBoundary;
            _axis = axis;
            _direction = direction;
            _planeValue = planeValue;
            _detectorController = new DetectorController(new List<IDetector>());
            Name = name;
            VirtualBoundaryType = type;
        }       

        /// <summary>
        /// Creates a default instance of a PlanarTransmissionVB based on a plane at z=0, 
        /// exiting tissue (in direction of z decreasing)
        /// </summary>
        public PlanarTransmissionVirtualBoundary() 
            : this(
            dp => true,
            VirtualBoundaryAxisType.Z, 
            VirtualBoundaryDirectionType.Decreasing, 
            0.0,
            VirtualBoundaryType.PlanarTransmissionDomainTopBoundary,
            VirtualBoundaryType.PlanarTransmissionDomainTopBoundary.ToString())
        {
        }

        public IDetectorController DetectorController { get { return _detectorController; } set { _detectorController = value; } }
        public string Name { get; set; }
        public VirtualBoundaryType VirtualBoundaryType { get; set; }
        public PhotonStateType PhotonStateType { get; private set; }
        public Predicate<PhotonDataPoint> WillHitBoundary { get; set; }

        /// <summary>
        /// Finds the distance to the virtual boundary given direction of VB and photon
        /// </summary>
        /// <param name="photon"></param>
        public double GetDistanceToVirtualBoundary(PhotonDataPoint dp)
        {
            double distanceToBoundary = double.PositiveInfinity;
            if (dp.Direction.Uz == 0.0)
            {
                return double.PositiveInfinity;
            }
            // going "up" in negative z-direction
            bool goingUp = dp.Direction.Uz < 0.0;
            // check that photon is directed in direction of VB
            if ((goingUp && (_direction == VirtualBoundaryDirectionType.Decreasing)) ||
                !goingUp && (_direction == VirtualBoundaryDirectionType.Increasing))
            {
                // calculate distance to boundary based on z-projection of photon trajectory
                distanceToBoundary = (_planeValue - dp.Position.Z) / dp.Direction.Uz;
            }
            return distanceToBoundary;
        }

    }
}
