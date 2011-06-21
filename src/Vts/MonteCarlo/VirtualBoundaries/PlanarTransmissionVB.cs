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
    public class PlanarTransmissionVB : IVirtualBoundary
    {
        private VirtualBoundaryAxisType _axis;
        private double _planeValue;
        private VirtualBoundaryDirectionType _direction;
        private IDetectorController _detectorController;
        /// <summary>
        /// Creates an instance of a plane tranmission virtual boundary in direction given
        /// </summary>
        public PlanarTransmissionVB(
            VirtualBoundaryAxisType axis,
            VirtualBoundaryDirectionType direction,
            double planeValue)
        {
            _axis = axis;
            _direction = direction;
            _planeValue = planeValue;
            _detectorController = new DetectorController(null);
        }       

        /// <summary>
        /// Creates a default instance of a PlanarTransmissionVB based on a plane at z=0, 
        /// exiting tissue (in direction of z decreasing)
        /// </summary>
        public PlanarTransmissionVB() 
            : this(
            VirtualBoundaryAxisType.Z, 
            VirtualBoundaryDirectionType.Decreasing, 
            0.0)
        {
        }

        public IDetectorController DetectorController { get { return _detectorController; } set { _detectorController = value; } }
       
        /// <summary>
        /// Finds the distance to the virtual boundary 
        /// </summary>
        /// <param name="photon"></param>
        public double GetDistanceToVirtualBoundary(Photon photon)
        {
            if (photon.DP.Direction.Uz == 0.0)
            {
                return double.PositiveInfinity;
            }
            // THE FOLLOWING NEEDS WORK TO BE CORRECT
            // going "up" in negative z-direction
            bool goingUp = photon.DP.Direction.Uz < 0.0;

            // calculate distance to boundary based on z-projection of photon trajectory
            double distanceToBoundary = (_planeValue - photon.DP.Position.Z) / photon.DP.Direction.Uz;

            return distanceToBoundary;
        }

    }
}
