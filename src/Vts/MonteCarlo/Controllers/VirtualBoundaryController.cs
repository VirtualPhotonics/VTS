using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.IO;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.VirtualBoundaries;

namespace Vts.MonteCarlo.Controllers
{
    public class VirtualBoundaryController
    {
        private IList<IVirtualBoundary> _virtualBoundaries;
        //private IList<ISurfaceDetector> _surfaceDetectors;
        //private IList<IVolumeDetector> _volumeDetectors;

        public VirtualBoundaryController(
            IList<IVirtualBoundary> virtualBoundaries)
        {
            _virtualBoundaries = virtualBoundaries;
        }

        public IList<IVirtualBoundary> VirtualBoundaries { get { return _virtualBoundaries; } set { _virtualBoundaries = value; } }
        public IVirtualBoundary ClosestVirtualBoundary { get; set; }

        // the following handles a list of VBs
        public IVirtualBoundary GetClosestVirtualBoundary(PhotonDataPoint dp, out double distance)
        {
            IVirtualBoundary vb = null;
            distance = double.PositiveInfinity;
            if (_virtualBoundaries != null && _virtualBoundaries.Count > 0)
            {
                foreach (var virtualBoundary in _virtualBoundaries)
                {
                    // each VB takes direction of VB into consideration when determining distance
                    var distanceToVB = virtualBoundary.GetDistanceToVirtualBoundary(dp);
                    if (distanceToVB < distance)
                    {
                        distance = distanceToVB;
                        vb = virtualBoundary;
                    }
                }
            }
  
            ClosestVirtualBoundary = vb;
            return vb;
        }

        //public void TallyToSurfaceDetectors(PhotonDataPoint dp)
        //{
        //    foreach (var vb in _virtualBoundaries)
        //    {
        //        if (dp.StateFlag.Has(vb.PhotonStateType))
        //        {
        //            _surfaceDetectors =
        //                (from detector in vb.DetectorController.Detectors
        //                 where detector.TallyType.IsSurfaceTally()
        //                 select (ISurfaceDetector)detector).ToArray();
        //            SurfaceTally(dp);
        //        }
        //    }
        //}

        //public void TallyToVolumeDetectors(PhotonHistory history)
        //{
        //    //var lastDP = history.HistoryData.Last();
        //    foreach (var vb in _virtualBoundaries)
        //    {
        //        if ((vb.VirtualBoundaryType == VirtualBoundaryType.GenericVolumeBoundary) ||
        //            (vb.VirtualBoundaryType == VirtualBoundaryType.Dosimetry))
        //        {
        //            _volumeDetectors =
        //                (from detector in vb.DetectorController.Detectors
        //                    where detector.TallyType.IsVolumeTally()
        //                    select (IVolumeDetector)detector).ToArray();
        //            VolumeTally(history);
        //        }
        //    }
        //}

        //public void SurfaceTally(PhotonDataPoint dp)
        //{
        //    foreach (var detector in _surfaceDetectors)
        //    {
        //        //if (tally.ContainsPoint(dp))
        //            detector.Tally(dp);
        //    }
        //}

        //public void VolumeTally(PhotonHistory history)
        //{
        //    // loop through the photon history. history tallies require information 
        //    // from previous and "current" collision points (including pseudo-collisions)
        //    PhotonDataPoint previousDP = history.HistoryData.First();
        //    foreach (PhotonDataPoint dp in history.HistoryData.Skip(1))
        //    {
        //        foreach (var detector in _volumeDetectors)
        //        {
        //            detector.Tally(previousDP, dp);
        //        }
        //        previousDP = dp;
        //    }
        //}

    }
}
