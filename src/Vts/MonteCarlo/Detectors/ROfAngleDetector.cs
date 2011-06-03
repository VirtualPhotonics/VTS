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
        private bool _tallySecondMoment;
        /// <summary>
        /// Returns an instance of RDiffuseDetector
        /// </summary>
        public ROfAngleDetector(DoubleRange angle, bool tallySecondMoment, String name)
        {
            Angle = angle;
            Mean = new double[Angle.Count - 1];
            _tallySecondMoment = tallySecondMoment;
            if (_tallySecondMoment)
            {
                SecondMoment = new double[Angle.Count - 1];
            }
            else
            {
                SecondMoment = null;
            }
            TallyType = TallyType.ROfAngle;
            Name = name;
            TallyCount = 0;
        }
        
        /// <summary>
        /// Returns a default instance of ROfAngleDetector (for serialization purposes only)
        /// </summary>
        public ROfAngleDetector()
            : this(new DoubleRange(), true, TallyType.ROfAngle.ToString())
        {
        }
        [IgnoreDataMember]
        public double[] Mean { get; set; }

        [IgnoreDataMember]
        public double[] SecondMoment { get; set; }

        public TallyType TallyType { get; set; }

        public String Name { get; set; }

        public long TallyCount { get; set; }

        public DoubleRange Angle { get; set; }

        public void Tally(PhotonDataPoint dp)
        {
            var ia = DetectorBinning.WhichBin(Math.Acos(dp.Direction.Uz), Angle.Count - 1, Angle.Delta, Angle.Start);

            Mean[ia] += dp.Weight;
            if (_tallySecondMoment)
            {
                SecondMoment[ia] += dp.Weight * dp.Weight;
            }
            TallyCount++;
        }

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Angle.Delta;
            for (int ia = 0; ia < Angle.Count - 1; ia++)
            {
                var areaNorm = Math.Sin((ia + 0.5) * Angle.Delta) * normalizationFactor;
                Mean[ia] /= areaNorm * numPhotons;
                if (_tallySecondMoment)
                {
                    SecondMoment[ia] /= areaNorm * areaNorm * numPhotons;
                }
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutTop);
        }

    }
}
