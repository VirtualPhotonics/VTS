using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    [KnownType(typeof(ROfRhoDetector))]
    /// <summary>
    /// Implements ITerminationTally<double[]>.  Tally for reflectance as a function 
    /// of Rho.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class ROfRhoDetector : ITerminationDetector<double[]>
    {
        /// <summary>
        /// Returns an instance of ROfRhoDetector
        /// </summary>
        /// <param name="rho"></param>
        public ROfRhoDetector(DoubleRange rho, String name)
        {
            Rho = rho;
            Mean = new double[Rho.Count - 1];
            SecondMoment = new double[Rho.Count - 1];
            TallyType = TallyType.ROfRho;
            Name = name;
            TallyCount = 0;
        }

        /// <summary>
        ///  Returns a default instance of ROfRhoDetector (for serialization purposes only)
        /// </summary>
        public ROfRhoDetector()
            : this(new DoubleRange(), TallyType.ROfRho.ToString())
        {
        }

        [IgnoreDataMember]
        public double[] Mean { get; set; }

        [IgnoreDataMember]
        public double[] SecondMoment { get; set; }

        public TallyType TallyType { get; set; }

        public String Name { get; set; }

        public long TallyCount { get; set; }

        public DoubleRange Rho { get; set; }

        public void Tally(PhotonDataPoint dp)
        {
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);

            Mean[ir] += dp.Weight;
            SecondMoment[ir] += dp.Weight * dp.Weight;
            TallyCount++;
        }

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta * Rho.Delta;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                Mean[ir] /= (ir + 0.5) * normalizationFactor * numPhotons;
                SecondMoment[ir] /= (ir + 0.5) * normalizationFactor *
                    (ir + 0.5) * normalizationFactor * numPhotons;
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutTop);
        }

    }
}
