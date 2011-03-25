using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    [KnownType(typeof(ROfRhoAndAngleDetector))]
    /// <summary>
    /// Implements ITerminationDetector<double[,]>.  Tally for reflectance as a function 
    /// of Rho and Angle.
    /// This works for Analog, DAW and CAW processing.
    /// </summary>
    public class ROfRhoAndAngleDetector : ITerminationDetector<double[,]>
    {
        /// <summary>
        /// Returns an instance of ROfRhoAndAngleDetector
        /// </summary>
        /// <param name="rho"></param>
        /// <param name="angle"></param>
        public ROfRhoAndAngleDetector(DoubleRange rho, DoubleRange angle, String name)
        {
            Rho = rho;
            Angle = angle;
            Mean = new double[Rho.Count - 1, Angle.Count];
            SecondMoment = new double[Rho.Count - 1, Angle.Count];
            TallyType = TallyType.ROfRhoAndAngle;
            Name = name;
            TallyCount = 0;
        }

        /// <summary>
        /// Returns a default instance of ROfRhoAndAngleDetector (for serialization purposes only)
        /// </summary>
        public ROfRhoAndAngleDetector()
            : this(new DoubleRange(), new DoubleRange(), TallyType.ROfRhoAndAngle.ToString())
        {
        }

        [IgnoreDataMember]
        public double[,] Mean { get; set; }

        [IgnoreDataMember]
        public double[,] SecondMoment { get; set; }

        public TallyType TallyType { get; set; }

        public long TallyCount { get; set; }

        public String Name { get; set; }

        public DoubleRange Rho { get; set; }

        public DoubleRange Angle { get; set; }

        public virtual void Tally(PhotonDataPoint dp)
        {
            var ia = DetectorBinning.WhichBin(Math.Acos(dp.Direction.Uz), Angle.Count - 1, Angle.Delta, Angle.Start);
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);

            Mean[ir, ia] += dp.Weight;
            SecondMoment[ir, ia] += dp.Weight * dp.Weight;
            TallyCount++;
        }

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta * Rho.Delta * 2.0 * Math.PI * Angle.Delta * numPhotons;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (int ia = 0; ia < Angle.Count; ia++)
                {
                    Mean[ir, ia] /= (ir + 0.5) * Math.Sin((ia + 0.5) * Angle.Delta) * normalizationFactor;
                }
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutTop);
        }

    }
}
