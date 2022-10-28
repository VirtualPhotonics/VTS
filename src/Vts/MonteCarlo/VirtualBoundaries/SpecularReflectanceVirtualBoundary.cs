using System;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.VirtualBoundaries
{
    /// <summary>
    /// Implements IVirtualBoundary.  Used to capture specular reflectance detectors
    /// </summary>
    public class SpecularReflectanceVirtualBoundary : IVirtualBoundary
    {
        private readonly double _zPlanePosition;

        /// <summary>
        /// specular reflectance virtual boundary
        /// </summary>
        /// <param name="tissue">ITissue</param>
        /// <param name="detectorController">IDetectorController</param>
        /// <param name="name">string name</param>
        public SpecularReflectanceVirtualBoundary(ITissue tissue, IDetectorController detectorController, string name)
        {
            _zPlanePosition = ((LayerTissueRegion)tissue.Regions[0]).ZRange.Stop;

            WillHitBoundary = dp =>
                        dp.StateFlag.HasFlag(PhotonStateType.PseudoReflectedTissueBoundary) &&
                        dp.Direction.Uz < 0 &&
                        Math.Abs(dp.Position.Z - _zPlanePosition) < 10E-16;

            VirtualBoundaryType = VirtualBoundaryType.SpecularReflectance;
            PhotonStateType = PhotonStateType.PseudoSpecularReflectanceVirtualBoundary;

            DetectorController = detectorController;

            Name = name;
        }       


        /// <summary>
        /// virtual boundary type identifier
        /// </summary>
        public VirtualBoundaryType VirtualBoundaryType { get; }
        /// <summary>
        /// photon state type
        /// </summary>
        public PhotonStateType PhotonStateType { get; }
        /// <summary>
        /// string name of VB
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// predicate of PhotonDataPoint whether photon will hit VB
        /// </summary>
        public Predicate<PhotonDataPoint> WillHitBoundary { get; }
        /// <summary>
        /// IDetectorController
        /// </summary>
        public IDetectorController DetectorController { get; }

        /// <summary>
        /// Finds the distance to the virtual boundary given direction of VB and photon
        /// </summary>
        /// <param name="dp">PhotonDataPoint</param>
        /// <returns>distance to VB</returns>
        public double GetDistanceToVirtualBoundary(PhotonDataPoint dp)
        {
            var distanceToBoundary = double.PositiveInfinity;

            // check if VB not applied
            if (!dp.StateFlag.HasFlag(PhotonStateType.PseudoSpecularTissueBoundary) ||
                dp.Direction.Uz >= 0.0) // if specular Uz will be negative
            {
                return distanceToBoundary;
            }
            // VB applies
            distanceToBoundary = (_zPlanePosition - dp.Position.Z) / dp.Direction.Uz;
          
            return distanceToBoundary;
        }

    }
}