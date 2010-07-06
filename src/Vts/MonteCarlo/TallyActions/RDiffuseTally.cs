using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.TallyActions
{
    /// <summary>
    /// Implements ITally<double>.  Tally for diffuse reflectance.
    /// </summary>
    public class RDiffuseTally : ITally<double>
    {
        //private double _rDiffuse;
        //private double _rDiffuseSecondMoment;

        public RDiffuseTally()
        {
            Mean = 0;
            SecondMoment = 0;
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutTop);
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
        public double Mean { get; set; }
        public double SecondMoment { get; set; }

        //public double Mean
        //{
        //    get { return _rDiffuse; }
        //}
        //public double SecondMoment 
        //{
        //    get { return _rDiffuseSecondMoment; }
        //}
    }
}
