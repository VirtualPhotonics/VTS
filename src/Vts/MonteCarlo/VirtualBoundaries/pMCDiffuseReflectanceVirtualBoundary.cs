using System;
using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.Controllers;

namespace Vts.MonteCarlo.VirtualBoundaries
{
    /// <summary>
    /// Implements IVirtualBoundary.  Used to capture all diffuse reflectance detectors
    /// </summary>
    public class pMCDiffuseReflectanceVirtualBoundary : IVirtualBoundary
    {
        private ISurfaceDetectorController _detectorController;
        private double _zPlanePosition;

        /// <summary>
        /// Creates an instance of a reflectance VB
        /// </summary>
        public pMCDiffuseReflectanceVirtualBoundary(ITissue tissue, ISurfaceDetectorController detectorController, string name)
        {
            _zPlanePosition = ((LayerRegion)tissue.Regions[0]).ZRange.Stop;

            WillHitBoundary = dp =>
                        dp.StateFlag.Has(PhotonStateType.PseudoReflectedTissueBoundary) &&
                        dp.Direction.Uz < 0 &&
                        Math.Abs(dp.Position.Z - _zPlanePosition) < 10E-16;

            VirtualBoundaryType = VirtualBoundaryType.pMCDiffuseReflectance;
            PhotonStateType = PhotonStateType.PseudoDiffuseReflectanceVirtualBoundary;

            _detectorController = detectorController;

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
        public IDetectorController DetectorController { get { return _detectorController; } }

        /// <summary>
        /// Finds the distance to the virtual boundary given direction of VB and photon
        /// </summary>
        /// <param name="photon"></param>
        public double GetDistanceToVirtualBoundary(PhotonDataPoint dp)
        {
            double distanceToBoundary = double.PositiveInfinity;
            // check if VB not applied
            if (!dp.StateFlag.Has(PhotonStateType.PseudoReflectedTissueBoundary) ||
                dp.Direction.Uz >= 0.0)
            {
                return distanceToBoundary;
            }
            // VB applies
            distanceToBoundary = (_zPlanePosition - dp.Position.Z) / dp.Direction.Uz;
            return distanceToBoundary;
        }

    }
}
