using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    public interface IDetectorController
    {
        IList<IDetector> Detectors { get; set; }
        //void TerminationTally(PhotonDataPoint dp);
        //void HistoryTally(PhotonHistory history);
        void NormalizeDetectors(long N);
        void SetOutputArrays(Output output);
    }
}
