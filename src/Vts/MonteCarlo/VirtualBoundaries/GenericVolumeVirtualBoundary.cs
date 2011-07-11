using System;
using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.Controllers;

namespace Vts.MonteCarlo.VirtualBoundaries
{
    /// <summary>
    /// Implements IVirtualBoundary.  Used to capture all internal volume detectors
    /// </summary>
    public class GenericVolumeVirtualBoundary : IVolumeVirtualBoundary
    {
        private IVolumeDetectorController _detectorController;
        /// <summary>
        /// Creates an instance of a volume virtual boundary
        /// </summary>
        public GenericVolumeVirtualBoundary(ITissue tissue, IDetectorController detectorController, string name)
        {
            //_zPlanePosition = ((LayerRegion)tissue.Regions[0]).ZRange.Stop;

            //WillHitBoundary = dp =>
            //            dp.StateFlag.Has(PhotonStateType.Transmitted) &&
            //            dp.Direction.Uz < 0 &&
            //            Math.Abs(dp.Position.Z - _zPlanePosition) < 10E-16;

            VirtualBoundaryType = VirtualBoundaryType.GenericVolumeBoundary;
            PhotonStateType = PhotonStateType.PseudoGenericVirtualBoundary;

            DetectorController = (IVolumeDetectorController)detectorController;

            Name = name;
        }      

        ///// <summary>
        ///// Creates a default instance of a GenericVolumeVB 
        ///// </summary>
        //public GenericVolumeVirtualBoundary() 
        //    : this(null, null, null)
        //{
        //}

        public VirtualBoundaryType VirtualBoundaryType { get; private set; }
        public PhotonStateType PhotonStateType { get; private set; }
        public string Name { get; private set; }
        public Predicate<PhotonDataPoint> WillHitBoundary { get; private set; }
        public IVolumeDetectorController DetectorController { get; private set; }

        /// <summary>
        /// Finds the distance to the virtual boundary 
        /// </summary>
        /// <param name="photon"></param>
        public double GetDistanceToVirtualBoundary(PhotonDataPoint dp)
        {
            // not sure of following, could be zero
            return double.PositiveInfinity;
        }

    }
}
