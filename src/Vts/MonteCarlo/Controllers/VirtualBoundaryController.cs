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
                //if ((dp.StateFlag.Has(PhotonStateType.PseudoDiffuseReflectanceVirtualBoundary) &&
                //    vb.PhotonStateType == PhotonStateType.PseudoDiffuseReflectanceVirtualBoundary) ||
                //    (dp.StateFlag.Has(PhotonStateType.PseudoDiffuseTransmittanceVirtualBoundary) &&
                //    vb.PhotonStateType == PhotonStateType.PseudoDiffuseTransmittanceVirtualBoundary))
                if (dp.StateFlag.Has(vb.PhotonStateType))
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

    }
}
