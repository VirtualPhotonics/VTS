using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    [KnownType(typeof(ROfAngleDetector))]
    /// <summary>
    /// Implements ITerminationDetector<double[]>.  Tally for reflectance as a function 
    /// of Angle.
    /// This works for Analog, DAW and CAW.
    /// </summary>
    public class ROfAngleDetector : ITerminationDetector<double[]>
    {
        /// <summary>
        /// Returns an instance of RDiffuseDetector
        /// </summary>
        public ROfAngleDetector(DoubleRange angle)
        {
            Angle = angle;
            Mean = new double[Angle.Count - 1];
            SecondMoment = new double[Angle.Count - 1];
            TallyType = TallyType.ROfAngle;
            TallyCount = 0;
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
            for (int ia = 0; ia < Angle.Count - 1; ia++)
            {
                Mean[ia] /= Math.Sin((ia + 0.5) * Angle.Delta) * normalizationFactor;
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutTop);
        }

    }
}
