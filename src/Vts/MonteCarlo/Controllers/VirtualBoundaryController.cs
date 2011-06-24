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
        private IList<ITerminationDetector> _terminationDetectors;
        private IList<IHistoryDetector> _historyDetectors;

        public VirtualBoundaryController(
            IList<IVirtualBoundary> virtualBoundaries)
        {
            _virtualBoundaries = virtualBoundaries;
        }

        public IList<IVirtualBoundary> VirtualBoundaries { get { return _virtualBoundaries; } }

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

            return vb;
        }

        public void TallyToTerminationDetectors(PhotonDataPoint dp)
        {
            foreach (var vb in _virtualBoundaries)
            {
                if ((dp.StateFlag.Has(PhotonStateType.PseudoDiffuseReflectanceVirtualBoundary) &&
                    vb.PhotonStateType == PhotonStateType.PseudoDiffuseReflectanceVirtualBoundary) ||
                    (dp.StateFlag.Has(PhotonStateType.PseudoDiffuseTransmittanceVirtualBoundary) &&
                    vb.PhotonStateType == PhotonStateType.PseudoDiffuseTransmittanceVirtualBoundary))
                {
                    _terminationDetectors =
                        (from detector in vb.DetectorController.Detectors
                         where detector.TallyType.IsTerminationTally()
                         select (ITerminationDetector)detector).ToArray();
                    TerminationTally(dp);
                }
            }
        }

        public void TallyToHistoryDetectors(PhotonHistory history)
        {
            //var lastDP = history.HistoryData.Last();
            foreach (var vb in _virtualBoundaries)
            {
                if (vb.VirtualBoundaryType == VirtualBoundaryType.GenericVolumeBoundary)  
                {
                    _historyDetectors =
                        (from detector in vb.DetectorController.Detectors
                            where detector.TallyType.IsHistoryTally()
                            select (IHistoryDetector)detector).ToArray();
                    HistoryTally(history);
                }
            }
        }

        public void TerminationTally(PhotonDataPoint dp)
        {
            foreach (var tally in _terminationDetectors)
            {
                //if (tally.ContainsPoint(dp))
                    tally.Tally(dp);
            }
        }

        public void HistoryTally(PhotonHistory history)
        {
            // loop through the photon history. history tallies require information 
            // from previous and "current" collision points (including pseudo-collisions)
            PhotonDataPoint previousDP = history.HistoryData.First();
            foreach (PhotonDataPoint dp in history.HistoryData.Skip(1))
            {
                foreach (var tally in _historyDetectors)
                {
                    tally.Tally(previousDP, dp);
                }
                previousDP = dp;
            }
        }

        //public bool ListenToPhotonStateType(PhotonDataPoint dp)
        //{
        //    bool virtualBoundary = false;
        //    // check if PST and VB agree
        //    foreach (var vb in _virtualBoundaries)
        //    {
        //        virtualBoundary = vb.WillHitBoundary(dp);
        //        switch (vb.VirtualBoundaryType)
        //        {
        //                // these cases would be in specific VB class and based on direction too
        //            case VirtualBoundaryType.PlanarTransmissionDomainTopBoundary:
        //                if (dp.StateFlag.Has(PhotonStateType.PseudoTransmissionDomainTopBoundary))
        //                    virtualBoundary = true;
        //                break;
        //            case VirtualBoundaryType.DiffuseTranmittance:
        //                if (dp.StateFlag.Has(PhotonStateType.PseudoTransmissionDomainBottomBoundary))
        //                    virtualBoundary = true;
        //                break;
        //            case VirtualBoundaryType.DiffuseReflectance:
        //                if (dp.StateFlag.Has(PhotonStateType.PseudoReflectionDomainTopBoundary))
        //                    virtualBoundary = true;
        //                break;
        //            case VirtualBoundaryType.PlanarReflectionDomainBottomBoundary:
        //                if (dp.StateFlag.Has(PhotonStateType.PseudoReflectionDomainBottomBoundary))
        //                    virtualBoundary = true;
        //                break;
        //            case VirtualBoundaryType.PlanarTransmissionInternalBoundary:
        //                if (dp.StateFlag.Has(PhotonStateType.PseudoTransmissionInternalBoundary))
        //                    virtualBoundary = true;
        //                break;
        //            case VirtualBoundaryType.PlanarReflectionInternalBoundary:
        //                if (dp.StateFlag.Has(PhotonStateType.PseudoReflectionInternalBoundary))
        //                    virtualBoundary = true;
        //                break;
        //        }
        //    }
        //    return virtualBoundary;
        //}
    }
}
