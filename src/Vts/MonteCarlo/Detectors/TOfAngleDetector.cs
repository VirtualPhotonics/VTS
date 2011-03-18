using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements ITerminationTally<double[]>.  Tally for transmittance as a function 
    /// of Angle. 
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class TOfAngleDetector : ITerminationDetector<double[]>
    {
        /// <summary>
        /// Returns an instance of TOfAngleDetector
        /// </summary>
        /// <param name="angle"></param>
        public TOfAngleDetector(DoubleRange angle)
        {
            Angle = angle;
            Mean = new double[Angle.Count];
            SecondMoment = new double[Angle.Count];
            TallyType = TallyType.TOfAngle;
            TallyCount = 0;
        }

        /// <summary>
        /// Returns a default instance of TDiffuseDetector (for serialization purposes only)
        /// </summary>
        public TOfAngleDetector()
            : this(new DoubleRange())
        {
        }

        [IgnoreDataMember]
        public double[] Mean { get; set; }

        [IgnoreDataMember]
        public double[] SecondMoment { get; set; }

        public TallyType TallyType { get; set; }

        public long TallyCount { get; set; }

        public DoubleRange Angle { get; set; }

        public void Tally(PhotonDataPoint dp)
        {
            var ia = DetectorBinning.WhichBin(Math.Acos(dp.Direction.Uz), Angle.Count - 1, Angle.Delta, Angle.Start);

            Mean[ia] += dp.Weight;
            SecondMoment[ia] += dp.Weight * dp.Weight;
            TallyCount++;

        }

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Angle.Delta * numPhotons;
            for (int ia = 0; ia < Angle.Count; ia++)
            {
                Mean[ia] /= Math.Sin((ia + 0.5) * Angle.Delta) * normalizationFactor;
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutBottom);
        }
    }
}
