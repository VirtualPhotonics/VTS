using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    [KnownType(typeof(TOfRhoAndAngleDetector))]
    /// <summary>
    /// Implements ITerminationTally<double[,]>.  Tally for transmittance as a function 
    /// of Rho and Angle.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class TOfRhoAndAngleDetector : ITerminationDetector<double[,]>
    {
        /// <summary>
        /// Returns an instance of TOfRhoAndAngleDetector
        /// </summary>
        /// <param name="rho"></param>
        /// <param name="angle"></param>
        public TOfRhoAndAngleDetector(DoubleRange rho, DoubleRange angle, String name)
        {
            Rho = rho;
            Angle = angle;
            Mean = new double[Rho.Count - 1, Angle.Count - 1];
            SecondMoment = new double[Rho.Count - 1, Angle.Count - 1];
            TallyType = TallyType.TOfRhoAndAngle;
            Name = name;
            TallyCount = 0;
        }

        /// <summary>
        /// Returns a default instance of TOfRhoAndAngleDetector (for serialization purposes only)
        /// </summary>
        public TOfRhoAndAngleDetector()
            : this(new DoubleRange(), new DoubleRange(), TallyType.TOfRhoAndAngle.ToString())
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

        public DoubleRange Angle { get; set; }

        public void Tally(PhotonDataPoint dp)
        {
            var ia = DetectorBinning.WhichBin(Math.Acos(dp.Direction.Uz), Angle.Count - 1, Angle.Delta, 0);
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);

            Mean[ir, ia] += dp.Weight;
            SecondMoment[ir, ia] += dp.Weight * dp.Weight;
            TallyCount++;
        }

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = numPhotons * 2.0 * Math.PI * Rho.Delta * Rho.Delta * 2.0 * Math.PI * Angle.Delta;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (int ia = 0; ia < Angle.Count - 1; ia++)
                {
                    Mean[ir, ia] /= (ir + 0.5) * Math.Sin((ia + 0.5) * Angle.Delta) * normalizationFactor;
                }
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutBottom);
        }

    }
}
