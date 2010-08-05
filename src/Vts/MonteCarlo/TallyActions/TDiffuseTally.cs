using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.TallyActions
{
    /// <summary>
    /// Implements ITerminationTally<double>.  Tally for diffuse transmittance.
    /// </summary>
    public class TDiffuseTally : ITerminationTally<double>
    {
        //private double _tDiffuse;
        //private double _tDiffuseSecondMoment;

        public TDiffuseTally()
        {
            Mean = 0.0;
            SecondMoment = 0.0;
        }

        public double Mean { get; set; }
        public double SecondMoment { get; set; }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutBottom);
        }
        public void Tally(PhotonDataPoint dp, IList<OpticalProperties> ops)
        {
            Mean += dp.Weight;
            SecondMoment += dp.Weight * dp.Weight;
        }

        public void Normalize(long numPhotons)
        {
            Mean /= numPhotons;
        }

        //public double Mean 
        //{
        //    get { return _tDiffuse; }
        //}
        //public double SecondMoment
        //{
        //    get { return _tDiffuseSecondMoment; }
        //}
    }
}
