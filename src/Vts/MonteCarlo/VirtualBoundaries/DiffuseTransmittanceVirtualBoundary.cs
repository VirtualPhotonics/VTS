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
    /// Implements IVirtualBoundary.  Used to capture all diffuse transmittance detectors
    /// </summary>
    public class DiffuseTransmittanceVirtualBoundary : IVirtualBoundary
    {
        private double _zPlanePosition;

        /// <summary>
        /// Creates an instance of a plane tranmission virtual boundary in direction given
        /// </summary>
        public DiffuseTransmittanceVirtualBoundary(ITissue tissue, IDetectorController detectorController, string name)
        {
            _zPlanePosition = ((LayerRegion)tissue.Regions[tissue.Regions.Count - 1]).ZRange.Start;

            WillHitBoundary = dp =>
                        dp.StateFlag.Has(PhotonStateType.PseudoTransmittedTissueBoundary) &&
                        dp.Direction.Uz > 0 &&
                        Math.Abs(dp.Position.Z - _zPlanePosition) < 10E-16;

            VirtualBoundaryType = VirtualBoundaryType.DiffuseTransmittance;
            PhotonStateType = PhotonStateType.PseudoDiffuseTransmittanceVirtualBoundary;

            DetectorController = (ISurfaceDetectorController)detectorController;

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

        public VirtualBoundaryType VirtualBoundaryType { get; private set; }
        public PhotonStateType PhotonStateType { get; private set; }
        public string Name { get; private set; }
        public Predicate<PhotonDataPoint> WillHitBoundary { get; private set; }
        public ISurfaceDetectorController DetectorController { get; private set; }

        /// <summary>
        /// Finds the distance to the virtual boundary given direction of VB and photon
        /// </summary>
        /// <param name="photon"></param>
        public double GetDistanceToVirtualBoundary(PhotonDataPoint dp)
        {
            double distanceToBoundary = double.PositiveInfinity;

            // check if VB applies
            if (!dp.StateFlag.Has(PhotonStateType.PseudoTransmittedTissueBoundary) ||
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
