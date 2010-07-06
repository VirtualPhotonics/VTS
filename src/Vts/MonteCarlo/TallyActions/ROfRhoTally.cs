using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.TallyActions
{
    /// <summary>
    /// Implements ITally<double[]>.  Tally for reflectance as a function 
    /// of Rho.
    /// </summary>
    public class ROfRhoTally : ITally<double[]>
    {
        private DoubleRange _rho;
        //private double[] _rOfRho;
        //private double[] _rOfRhoSecondMoment;

        public ROfRhoTally(DoubleRange rho)
        {
            _rho = rho;
            Mean = new double[_rho.Count];
            SecondMoment = new double[_rho.Count];
        }

        public double[] Mean { get; set; }
        public double[] SecondMoment { get; set; }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutTop);
        }
        public void Tally(PhotonDataPoint dp, IList<OpticalProperties> ops)
        {
            var totalTime = dp.SubRegionInfoList.Select((sub, i) =>
                DetectorBinning.GetTimeDelay(
                    sub.PathLength,
                    ops[i].N)
            ).Sum();

            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), _rho.Count, _rho.Delta, _rho.Start);

            Mean[ir] += dp.Weight;
            SecondMoment[ir] += dp.Weight * dp.Weight;
        }

        public void Normalize(long numPhotons)
        {
            for (int ir = 0; ir < _rho.Count; ir++)
            {
                Mean[ir] /= 2.0 * Math.PI * (ir + 0.5) * _rho.Delta * _rho.Delta * numPhotons;
            }
        }

        //public double[] Mean 
        //{
        //    get { return _rOfRho; }
        //}
        //public double[] SecondMoment
        //{
        //    get { return _rOfRhoSecondMoment; }
        //}
    }
}
