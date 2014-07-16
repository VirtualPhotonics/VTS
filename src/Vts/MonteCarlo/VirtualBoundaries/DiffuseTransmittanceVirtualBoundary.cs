using System;
using System.Linq;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.VirtualBoundaries
{
    /// <summary>
    /// Implements IVirtualBoundary.  Used to capture all diffuse transmittance detectors
    /// </summary>
    public class DiffuseTransmittanceVirtualBoundary : IVirtualBoundary
    {
        private IDetectorController _detectorController;
        private double _zPlanePosition;

        /// <summary>
        /// diffuse transmittance VB
        /// </summary>
        /// <param name="tissue">ITissue</param>
        /// <param name="detectorController">IDetectorController</param>
        /// <param name="name">string name</param>
        public DiffuseTransmittanceVirtualBoundary(ITissue tissue, IDetectorController detectorController, string name)
        {
            _zPlanePosition = ((LayerRegion)tissue.Regions.Where(r => r is LayerRegion).Last()).ZRange.Start;

            WillHitBoundary = dp =>
                        dp.StateFlag.HasFlag(PhotonStateType.PseudoTransmittedTissueBoundary) &&
                        dp.Direction.Uz > 0 &&
                        Math.Abs(dp.Position.Z - _zPlanePosition) < 10E-16;

            VirtualBoundaryType = VirtualBoundaryType.DiffuseTransmittance;
            PhotonStateType = PhotonStateType.PseudoDiffuseTransmittanceVirtualBoundary;

            _detectorController = detectorController;

            Name = name;
        }       

        ///// <summary>
        ///// Creates a default instance of a PlanarTransmissionVB based on a plane at z=0, 
        ///// exiting tissue (in direction of z decreasing)
        ///// </summary>
        //public DiffuseTransmittanceVirtualBoundary() 
        //    : this(null, null, null)
        //{
        //}

          /// <summary>
        /// VirtualBoundaryType enum indicating type of VB
        /// </summary>
        public VirtualBoundaryType VirtualBoundaryType { get; private set; }
        /// <summary>
        /// PhotonStateType enum indicating state of photon at this VB
        /// </summary>
        public PhotonStateType PhotonStateType { get; private set; }
        /// <summary>
        /// Name string of VB
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Predicate method to indicate if photon will hit VB boundary
        /// </summary>
        public Predicate<PhotonDataPoint> WillHitBoundary { get; private set; }
        /// <summary>
        /// IDetectorController specifying type of detector controller.
        /// </summary>
        public IDetectorController DetectorController { get { return _detectorController; } }

        /// <summary>
        /// Finds the distance to the virtual boundary given direction of VB and photon
        /// </summary>
        /// <param name="dp">photon data point</param>
        public double GetDistanceToVirtualBoundary(PhotonDataPoint dp)
        {
            double distanceToBoundary = double.PositiveInfinity;

            // check if VB applies
            if (!dp.StateFlag.HasFlag(PhotonStateType.PseudoTransmittedTissueBoundary) ||
                dp.Direction.Uz <= 0.0)
            {
                return distanceToBoundary;
            }
            // VB applies
            distanceToBoundary = (_zPlanePosition - dp.Position.Z) / dp.Direction.Uz;
            
            return distanceToBoundary;
        }

    }
}
