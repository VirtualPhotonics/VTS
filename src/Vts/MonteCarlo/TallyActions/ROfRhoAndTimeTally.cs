using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.TallyActions
{
    /// <summary>
    /// Implements ITally<double[,]>.  Tally for reflectance as a function 
    /// of Rho and Time.
    /// </summary>
    public class ROfRhoAndTimeTally : ITally<double[,]>
    {
        private DoubleRange _rho;
        private DoubleRange _time;
        //private double[,] _rOfRhoAndTime;
        //private double[,] _rOfRhoAndTimeSecondMoment;

        public ROfRhoAndTimeTally(DoubleRange rho, DoubleRange time)
        {
            _rho = rho;
            _time = time;
            Mean = new double[_rho.Count, _time.Count];
            SecondMoment = new double[_rho.Count, _time.Count];
        }

        public double[,] Mean { get; set; }
        public double[,] SecondMoment { get; set; }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutTop);
        }

        public virtual void Tally(PhotonDataPoint dp, IList<OpticalProperties> ops)
        {
            var totalTime = dp.SubRegionInfoList.Select((sub, i) =>
                DetectorBinning.GetTimeDelay(
                    sub.PathLength,
                    ops[i].N)
            ).Sum();

            var it = DetectorBinning.WhichBin(totalTime, _time.Count, _time.Delta, _time.Start);
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), _rho.Count, _rho.Delta, _rho.Start);

            Mean[ir, it] += dp.Weight;
            SecondMoment[ir, it] += dp.Weight * dp.Weight;
        }

        public void Normalize(long numPhotons)
        {
            for (int ir = 0; ir < _rho.Count; ir++)
            {
                for (int it = 0; it < _time.Count; it++)
                {
                    Mean[ir, it] /=
                        2.0 * Math.PI * (ir + 0.5) * _rho.Delta * _rho.Delta * _time.Delta * numPhotons;
                }
            }
        }

        //public double[,] Mean
        //{
        //    get { return _rOfRhoAndTime; }
        //}

        //public double[,] SecondMoment
        //{
        //    get { return _rOfRhoAndTimeSecondMoment; }
        //}
    }
}
