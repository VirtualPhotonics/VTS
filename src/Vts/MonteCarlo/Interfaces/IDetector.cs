using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    public interface IDetector
    {
        // note that the properties encompass Cartesian and Cylindrical Position systems
        List<TallyType> TallyTypeList { get; set; }
        List<ITally> TerminationITallyList { get; set; }
        List<ITally> HistoryITallyList { get; set; }
        DoubleRange Rho { get; set; }
        DoubleRange Angle { get; set; }
        DoubleRange Time { get; set; }
        DoubleRange Omega { get; set; }
        DoubleRange X { get; set; }
        DoubleRange Y { get; set; }
        DoubleRange Z { get; set; }
        void SetTallyActionLists();
        void TerminationTally(PhotonDataPoint dp);
        void NormalizeTalliesToOutput(long N, Output output);
        void SetOutputArrays(Output output);
        void HistoryTally(PhotonDataPoint dp);
    }
}
