using System;
using System.Linq;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.VirtualBoundaries
{
    /// <summary>
    /// Implements IVirtualBoundary.  Used to capture all diffuse transmittance detectors
    /// </summary>
    public class pMCDiffuseTransmittanceVirtualBoundary : IVirtualBoundary
    {
        private readonly double _zPlanePosition;

        /// <summary>
        /// class for perturbation Monte Carlo (pMC) diffuse transmittance virtual boundary
        /// </summary>
        /// <param name="tissue">ITissue</param>
        /// <param name="detectorController">IDetectorController</param>
        /// <param name="name">string name</param>
        public pMCDiffuseTransmittanceVirtualBoundary(ITissue tissue, IDetectorController detectorController, string name)
        {
            _zPlanePosition = ((LayerTissueRegion)tissue.Regions.Last(r => r is LayerTissueRegion)).ZRange.Start;

            WillHitBoundary = dp =>
                        dp.StateFlag.HasFlag(PhotonStateType.PseudoTransmittedTissueBoundary) &&
                        dp.Direction.Uz > 0 &&
                        Math.Abs(dp.Position.Z - _zPlanePosition) < 10E-16;

            VirtualBoundaryType = VirtualBoundaryType.pMCDiffuseTransmittance;
            PhotonStateType = PhotonStateType.PseudoDiffuseTransmittanceVirtualBoundary;

            DetectorController = detectorController;

            Name = name;
        }       

        /// <summary>
        /// VirtualBoundaryType
        /// </summary>
        public VirtualBoundaryType VirtualBoundaryType { get; }
        /// <summary>
        /// PhotonStateType
        /// </summary>
        public PhotonStateType PhotonStateType { get; }
        /// <summary>
        /// string Name
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Predicate of PhotonDataPoint
        /// </summary>
        public Predicate<PhotonDataPoint> WillHitBoundary { get; }
        /// <summary>
        /// IDetectorController
        /// </summary>
        public IDetectorController DetectorController { get; }

        /// <summary>
        /// Finds the distance to the virtual boundary given direction of VB and photon
        /// </summary>
        /// <param name="dp">photon data point</param>
        /// <returns>distance to virtual boundary</returns>
        public double GetDistanceToVirtualBoundary(PhotonDataPoint dp)
        {
            var distanceToBoundary = double.PositiveInfinity;
            // check if VB not applied
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
