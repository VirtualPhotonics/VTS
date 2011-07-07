using System;
using System.Linq;
using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo.VirtualBoundaries
{
    /// <summary>
    /// Implements IVirtualBoundary.  Used to capture specular reflectance detectors
    /// </summary>
    public class DosimetryVirtualBoundary : IVirtualBoundary
    {
        private double _zPlanePosition;

        /// <summary>
        /// Creates an instance of a plane tranmission virtual boundary in direction given
        /// </summary>
        public DosimetryVirtualBoundary(IList<IDetector> detectors, string name)
        {
            IDetector dosimetryDetector = detectors.Where(d => d.TallyType == TallyType.DosimetryOfRho).First();

            _zPlanePosition = ((DosimetryOfRhoDetector)dosimetryDetector).ZDepth;

            WillHitBoundary = dp =>
                        dp.StateFlag.Has(PhotonStateType.PseudoReflectedTissueBoundary) &&
                        dp.Direction.Uz > 0 &&
                        Math.Abs(dp.Position.Z - _zPlanePosition) < 10E-16;

            VirtualBoundaryType = VirtualBoundaryType.Dosimetry;
            PhotonStateType = PhotonStateType.PseudoDosimetryVirtualBoundary;

            DetectorController = DetectorControllerFactory.GetStandardDetectorController(detectors);

            Name = name;
        }       

        ///// <summary>
        ///// Creates a default instance of a DosimetryVB based on a plane at z=0, 
        ///// exiting tissue (in direction of z decreasing)
        ///// </summary>
        //public DosimetryVirtualBoundary() 
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

            // check if VB not applied
            if (!dp.StateFlag.Has(PhotonStateType.PseudoDosimetryTissueBoundary) ||
                dp.Direction.Uz <= 0.0)
            {
                return distanceToBoundary;
            }
            // VB applies
            distanceToBoundary = Math.Abs(_zPlanePosition - dp.Position.Z) / dp.Direction.Uz;
          
            return distanceToBoundary;
        }

    }
}