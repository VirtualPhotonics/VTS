using System;
using System.Linq;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.VirtualBoundaries
{
    /// <summary>
    /// Implements IVirtualBoundary.  Used to capture surface radiance detectors internal to the tissue
    /// </summary>
    public class InternalFiberVirtualBoundary : IVirtualBoundary
    {
        private IDetectorController _detectorController;
        private double _zPlanePosition;
        private double _radius;

        /// <summary>
        /// Radiance virtual boundary
        /// </summary>
        /// <param name="detectorController">IDetectorController</param>
        /// <param name="name">string name</param>
        public InternalFiberVirtualBoundary(IDetectorController detectorController, string name)
        {
            _detectorController = detectorController;

            // not sure following is best design
            //todo: revisit design(dc 6 / 10 / 12)
            IDetector submergedFiberDetector = DetectorController.Detectors.Where(
                d => d.TallyDetails.IsInternalFiberTally).FirstOrDefault();

            if (submergedFiberDetector != null)
            {
                _zPlanePosition = ((dynamic)submergedFiberDetector).Center.Z + ((dynamic)submergedFiberDetector).HeightZ / 2;
                _radius = ((dynamic) submergedFiberDetector).Radius;

                WillHitBoundary = dp =>
                    dp.StateFlag.HasFlag(PhotonStateType.PseudoReflectedTissueBoundary) &&
                    dp.Direction.Uz < 0 &&
                    Math.Abs(dp.Position.Z - _zPlanePosition) < 10E-16 &&
                    Math.Sqrt(dp.Position.X * dp.Position.X + dp.Position.Y * dp.Position.Y) < _radius;

                VirtualBoundaryType = VirtualBoundaryType.InternalFiber;
                PhotonStateType = PhotonStateType.PseudoInternalFiberVirtualBoundary;

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
        /// <param name="dp">photo data point</param> (
        public double GetDistanceToVirtualBoundary(PhotonDataPoint dp)
        {
            double distanceToBoundary = double.PositiveInfinity;

            // check if VB not applied // CKH CHECK IF WE USE PhotonStateType.Pseudo..VB
            //if (!dp.StateFlag.Has(PhotonStateType.PseudoRadianceVirtualBoundary) ||
            //    dp.Direction.Uz <= 0.0)

            // since no tissue boundary here, need other checks for whether VB is applied
            if ((dp.Direction.Uz >= 0.0) || (dp.Position.Z <= _zPlanePosition)) // >= is key here
            {
                return distanceToBoundary; // photon is pointed down or above fiber bottom cap
            }
            // check if photon intersects fiber bottom cap
            // project distance to plane
            var distanceToPlane = (_zPlanePosition - dp.Position.Z) / dp.Direction.Uz;
            var xIntersect = dp.Position.X + distanceToPlane * dp.Direction.Ux;
            var yIntersect = dp.Position.Y + distanceToPlane * dp.Direction.Uy;
            if (Math.Sqrt(xIntersect * xIntersect + yIntersect * yIntersect) < _radius)
            {
                return distanceToBoundary;
            }
            return double.PositiveInfinity; // return infinity                           
        }

    }
}