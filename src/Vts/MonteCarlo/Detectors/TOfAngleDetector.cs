using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    [KnownType(typeof(TOfAngleDetector))]
    /// <summary>
    /// Implements ITerminationTally<double[]>.  Tally for transmittance as a function 
    /// of Angle. 
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class TOfAngleDetector : ITerminationDetector<double[]>
    {
        private bool _tallySecondMoment;
        /// <summary>
        /// Returns an instance of TOfAngleDetector
        /// </summary>
        /// <param name="angle"></param>
        public TOfAngleDetector(DoubleRange angle, bool tallySecondMoment, String name)
        {
            Angle = angle;
            _tallySecondMoment = tallySecondMoment;
            Mean = new double[Angle.Count];
            if (_tallySecondMoment)
            {
                SecondMoment = new double[Angle.Count];
            }
            else
            {
                SecondMoment = null;
            }
            TallyType = TallyType.TOfAngle;
            Name = name;
            TallyCount = 0;
        }

        /// <summary>
        /// Returns a default instance of TDiffuseDetector (for serialization purposes only)
        /// </summary>
        public TOfAngleDetector()
            : this(new DoubleRange(), true, TallyType.TOfAngle.ToString())
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
            for (int ia = 0; ia < Angle.Count; ia++)
            {
                var areaNorm = Math.Sin((ia + 0.5) * Angle.Delta) * normalizationFactor;
                Mean[ia] /=  areaNorm * numPhotons;
                if (_tallySecondMoment)
                {
                    SecondMoment[ia] /= areaNorm * areaNorm * numPhotons;
                }
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutBottom);
        }
    }
}
