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
    /// Implements ITerminationDetector<double[]>.  Tally for reflectance as a function 
    /// of Angle.
    /// This works for Analog, DAW and CAW.
    /// </summary>
    public class ROfAngleDetector : ITerminationDetector<double[]>
    {
        private DoubleRange _angle;

        public ROfAngleDetector(DoubleRange angle)
        {
            _angle = angle;
            Mean = new double[_angle.Count - 1];
            SecondMoment = new double[_angle.Count - 1];
            TallyType = TallyType.ROfAngle;
        }


        [IgnoreDataMember]
        public double[] Mean { get; set; }

        [IgnoreDataMember]
        public double[] SecondMoment { get; set; }

        public TallyType TallyType { get; set; }

        public void Tally(PhotonDataPoint dp)
        {
            var ia = DetectorBinning.WhichBin(Math.Acos(dp.Direction.Uz), _angle.Count - 1, _angle.Delta, _angle.Start);

            Mean[ia] += dp.Weight;
            SecondMoment[ia] += dp.Weight * dp.Weight;
        }

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * _angle.Delta * numPhotons;
            for (int ia = 0; ia < _angle.Count - 1; ia++)
            {
                Mean[ia] /= Math.Sin((ia + 0.5) * _angle.Delta) * normalizationFactor;
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutTop);
        }

    }
}
