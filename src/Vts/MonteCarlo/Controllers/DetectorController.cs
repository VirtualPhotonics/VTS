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
    public class DetectorController : IDetectorController
    {
        private IList<IDetector> _detectors;
        //private IList<ITerminationDetector> _terminationDetectors;
        //private IList<IHistoryDetector> _historyDetectors;

        public DetectorController(
            IList<IDetector> detectors)
        {
            _detectors = detectors;

            //_terminationDetectors =
            //    (from detector in _detectors
            //     where detector.TallyType.IsTerminationTally()
            //     select (ITerminationDetector)detector).ToArray();

            //// DC what to do about history detectors for now?  I could make generic VB and add them to it.
            //if (detectors != null)
            //{
            //    _historyDetectors =
            //        (from detector in _detectors
            //         where detector.TallyType.IsHistoryTally()
            //         select (IHistoryDetector)detector).ToArray();
            //}

        }

        public IList<IDetector> Detectors { get { return _detectors; } }

        //public void TerminationTally(PhotonDataPoint dp)
        //{
        //    foreach (var tally in _terminationDetectors)
        //    {
        //        if (tally.ContainsPoint(dp))
        //            tally.Tally(dp);
        //    }
        //}

        //public void HistoryTally(PhotonHistory history)
        //{
        //    // loop through the photon history. history tallies require information 
        //    // from previous and "current" collision points (including pseudo-collisions)
        //    PhotonDataPoint previousDP = history.HistoryData.First();
        //    foreach (PhotonDataPoint dp in history.HistoryData.Skip(1))
        //    {
        //        foreach (var tally in _historyDetectors)
        //        {
        //            tally.Tally(previousDP, dp);
        //        }
        //        previousDP = dp;
        //    }
        //}

        public virtual void NormalizeDetectors(long N)
        {
            foreach (var detector in _detectors)
            {
                detector.Normalize(N);
            }
        }
    }
}
