using System;
using System.Collections.Generic;
using System.Linq;
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
        private DoubleRange _angle;

        public TOfAngleDetector(DoubleRange angle)
        {
            _angle = angle;
            Mean = new double[_angle.Count];
            SecondMoment = new double[_angle.Count];
            TallyType = TallyType.TOfAngle;
        }

        public double[] Mean { get; set; }
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
            for (int ia = 0; ia < _angle.Count; ia++)
            {
                Mean[ia] /= Math.Sin((ia + 0.5) * _angle.Delta) * normalizationFactor;
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutBottom);
        }
    }
}
