using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.Extensions;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Controllers;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.Factories;

namespace Vts.MonteCarlo.VirtualBoundaries
{
    /// <summary>
    /// Implements IVirtualBoundary.  Used to capture all reflectance/transmittance detectors
    /// </summary>
    public class DiffuseReflectanceVirtualBoundary : IVirtualBoundary
    {
        private double _zPlanePosition;
        private IDetectorController _detectorController;

        /// <summary>
        /// Creates an instance of a plane tranmission virtual boundary in direction given
        /// </summary>
        public DiffuseReflectanceVirtualBoundary(ITissue tissue, IList<IDetector> detectors, string name)
        {
            _zPlanePosition = ((LayerRegion)tissue.Regions[0]).ZRange.Stop;

            WillHitBoundary = dp =>
                        dp.StateFlag.Has(PhotonStateType.Transmitted) &&
                        dp.Direction.Uz < 0 &&
                        Math.Abs(dp.Position.Z - _zPlanePosition) < 10E-16;

            VirtualBoundaryType = VirtualBoundaryType.DiffuseReflectance;
            PhotonStateType = PhotonStateType.DiffuseReflectanceVirtualBoundary;

            _detectorController = DetectorControllerFactory.GetStandardDetectorController(detectors);

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

        public VirtualBoundaryType VirtualBoundaryType { get; private set; }
        public PhotonStateType PhotonStateType { get; private set; }
        public string Name { get; private set; }
        public Predicate<PhotonDataPoint> WillHitBoundary { get; private set; }
        public IDetectorController DetectorController { get; private set; }

        /// <summary>
        /// Finds the distance to the virtual boundary given direction of VB and photon
        /// </summary>
        /// <param name="photon"></param>
        public double GetDistanceToVirtualBoundary(PhotonDataPoint dp)
        {
            double distanceToBoundary = double.PositiveInfinity;

            if (!dp.StateFlag.Has(PhotonStateType.Transmitted) ||
                dp.Direction.Uz >= 0.0)
            {
                return distanceToBoundary;
            }

            //// going "up" in negative z-direction
            //bool goingUp = dp.Direction.Uz < 0.0;
            //// check that photon is directed in direction of VB
            //if ((goingUp && (_direction == VirtualBoundaryDirectionType.Decreasing)) ||
            //    !goingUp && (_direction == VirtualBoundaryDirectionType.Increasing))
            //{
                // calculate distance to boundary based on z-projection of photon trajectory
                distanceToBoundary = (_zPlanePosition - dp.Position.Z) / dp.Direction.Uz;
            //}
            return distanceToBoundary;
        }

    }
}
