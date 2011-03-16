using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements ITerminationTally<double[,]>.  Tally for reflectance as a function 
    /// of Rho and Time.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class ROfRhoAndTimeDetector : TallyBase, ITerminationDetector<double[,]>
    {
        private DoubleRange _rho;
        private DoubleRange _time;

        public ROfRhoAndTimeDetector(DoubleRange rho, DoubleRange time, ITissue tissue)
            : base(tissue)
        {
            _rho = rho;
            _time = time;
            Mean = new double[_rho.Count - 1, _time.Count - 1];
            SecondMoment = new double[_rho.Count - 1, _time.Count - 1];
            TallyType = TallyType.ROfRhoAndTime;
        }

        public double[,] Mean { get; set; }
        public double[,] SecondMoment { get; set; }
        public TallyType TallyType { get; set; }

        public virtual void Tally(PhotonDataPoint dp)
        {
            var it = DetectorBinning.WhichBin(dp.TotalTime, _time.Count - 1, _time.Delta, _time.Start);
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), _rho.Count - 1, _rho.Delta, _rho.Start);

            Mean[ir, it] += dp.Weight;
            SecondMoment[ir, it] += dp.Weight * dp.Weight;
        }

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * _rho.Delta * _rho.Delta * _time.Delta * numPhotons;
            for (int ir = 0; ir < _rho.Count - 1; ir++)
            {
                for (int it = 0; it < _time.Count - 1; it++)
                {
                    Mean[ir, it] /= (ir + 0.5) * normalizationFactor;
                }
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutTop);
        }

    }
}
