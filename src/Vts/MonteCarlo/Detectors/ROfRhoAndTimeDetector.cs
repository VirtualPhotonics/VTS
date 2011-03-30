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
        /// <summary>
        ///  Returns an instance of ROfRhoAndTimeDetector
        /// </summary>
        /// <param name="rho"></param>
        /// <param name="time"></param>
        /// <param name="tissue"></param>
        public ROfRhoAndTimeDetector(DoubleRange rho, DoubleRange time, String name)
        {
            Rho = rho;
            Time = time;
            Mean = new double[Rho.Count - 1, Time.Count - 1];
            SecondMoment = new double[Rho.Count - 1, Time.Count - 1];
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
            SecondMoment[ir, it] += dp.Weight * dp.Weight;
            TallyCount++;
        }

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta * Rho.Delta * Time.Delta * numPhotons;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (int it = 0; it < Time.Count - 1; it++)
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
