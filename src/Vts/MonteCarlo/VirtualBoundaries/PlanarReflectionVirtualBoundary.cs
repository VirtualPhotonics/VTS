using System.Collections.Generic;
using Vts.MonteCarlo.Controllers;

namespace Vts.MonteCarlo.VirtualBoundaries
{
    /// <summary>
    /// Implements IVirtualBoundary.  Used to capture specular detector for example
    /// </summary>
    public class PlanarReflectionVirtualBoundary : IVirtualBoundary
    {
        private VirtualBoundaryAxisType _axis;
        private double _planeValue;
        private VirtualBoundaryDirectionType _direction;
        private IDetectorController _detectorController;

        /// <summary>
        /// Creates an instance of a plane tranmission virtual boundary in direction given
        /// </summary>
        public PlanarReflectionVirtualBoundary(
            VirtualBoundaryAxisType axis,
            VirtualBoundaryDirectionType direction,
            double planeValue)
        {
            _axis = axis;
            _direction = direction;
            _planeValue = planeValue;
            _detectorController = new DetectorController(new List<IDetector>());
        }       

        /// <summary>
        /// Creates a default instance of a PlanarReflectionVB based on a plane at z=0, 
        /// reflecting off tissue (in direction of z decreasing)
        /// </summary>
        public PlanarReflectionVirtualBoundary() 
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
        // the following handles a list of VBs
        //public double GetDistanceToClosestVirtualBoundary(Photon photon)
        //{
        //    var distance = double.PositiveInfinity;

        //    //if (_virtualBoundaries != null && _virtualBoundaries.Count > 0)
        //    //{
        //    //    foreach (var virtualBoundary in _virtualBoundaries)
        //    //    {
        //    //        var distanceToVB = virtualBoundary.GetDistanceToBoundary(photon);

        //    //        if(distanceToVB <= distance)
        //    //        {
        //    //            distance = distanceToVB;
        //    //        }
        //    //    }
        //    //}

        //    return distance;
        //}
    }
}
