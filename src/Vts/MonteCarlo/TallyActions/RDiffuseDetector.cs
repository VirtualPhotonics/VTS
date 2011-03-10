using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.TallyActions
{
    /// <summary>
    /// Implements ITerminationDetector<double>.  Tally for diffuse reflectance.
    /// This implementation works for Analog, DAW and CAW.
    /// </summary>
    public class RDiffuseDetector : ITerminationDetector<double>
    {
        public RDiffuseDetector()
        {
            Mean = 0;
            SecondMoment = 0;
            TallyType = TallyType.RDiffuse;
        }

        public double Mean { get; set; }
        public double SecondMoment { get; set; }
        public TallyType TallyType { get; set; }

        public void Tally(PhotonDataPoint dp)
        {
            Mean += dp.Weight;
            SecondMoment += dp.Weight * dp.Weight;
        }

        public void Normalize(long numPhotons)
        {
            Mean /= numPhotons;
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutTop);
        }
    }
}
