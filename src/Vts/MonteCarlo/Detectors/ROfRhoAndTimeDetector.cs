using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Detectors
{ 
    [KnownType(typeof(ROfRhoAndTimeDetector))]
    /// <summary>
    /// Implements ITerminationTally<double[,]>.  Tally for reflectance as a function 
    /// of Rho and Time.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class ROfRhoAndTimeDetector : ITerminationDetector<double[,]>
    {
        private bool _tallySecondMoment;
        /// <summary>
        ///  Returns an instance of ROfRhoAndTimeDetector
        /// </summary>
        /// <param name="rho"></param>
        /// <param name="time"></param>
        /// <param name="tissue"></param>
        public ROfRhoAndTimeDetector(DoubleRange rho, DoubleRange time, bool tallySecondMoment, String name)
        {
            Rho = rho;
            Time = time;
            _tallySecondMoment = tallySecondMoment;
            Mean = new double[Rho.Count - 1, Time.Count - 1];
            if (_tallySecondMoment)
            {
                SecondMoment = new double[Rho.Count - 1, Time.Count - 1];
            }
            else
            {
                SecondMoment = null;
            }
            TallyType = TallyType.ROfRhoAndTime;
            Name = name;
            TallyCount = 0;
        }

        /// <summary>
        /// Returns a default instance of ROfRhoAndTimeDetector (for serialization purposes only)
        /// </summary>
        public ROfRhoAndTimeDetector()
            : this(
            new DoubleRange(),  
            new DoubleRange(),
            true,
            TallyType.ROfRhoAndTime.ToString())
        {
        }

        [IgnoreDataMember]
        public double[,] Mean { get; set; }

        [IgnoreDataMember]
        public double[,] SecondMoment { get; set; }

        public TallyType TallyType { get; set; }

        public String Name { get; set; }

        public long TallyCount { get; set; }

        public DoubleRange Rho { get; set; }

        public DoubleRange Time { get; set; }

        public virtual void Tally(PhotonDataPoint dp)
        {
            var it = DetectorBinning.WhichBin(dp.TotalTime, Time.Count - 1, Time.Delta, Time.Start);
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);

            Mean[ir, it] += dp.Weight;
            if (_tallySecondMoment)
            {
                SecondMoment[ir, it] += dp.Weight * dp.Weight;
            }
            TallyCount++;
        }

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta * Rho.Delta * Time.Delta;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (int it = 0; it < Time.Count - 1; it++)
                {
                    var areaNorm = (ir + 0.5) * normalizationFactor;
                    Mean[ir, it] /= areaNorm * numPhotons;
                    if (_tallySecondMoment)
                    {
                        SecondMoment[ir, it] /= areaNorm * areaNorm * numPhotons;
                    }
                }
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag.Has(PhotonStateType.ExitedOutTop));
        }

    }
}
