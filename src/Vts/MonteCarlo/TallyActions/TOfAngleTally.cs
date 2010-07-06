using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.TallyActions
{
    /// <summary>
    /// Implements ITally<double[]>.  Tally for transmittance as a function 
    /// of Angle.
    /// </summary>
    public class TOfAngleTally : ITally<double[]>
    {
        private DoubleRange _angle;
        //private double[] _tOfAngle;
        //private double[] _tOfAngleSecondMoment;

        public TOfAngleTally(DoubleRange angle)
        {
            _angle = angle;
            Mean = new double[_angle.Count];
            SecondMoment = new double[_angle.Count];
        }

        public double[] Mean { get; set; }
        public double[] SecondMoment { get; set; }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutBottom);
        }

        public void Tally(PhotonDataPoint dp, IList<OpticalProperties> ops)
        {
            var totalTime = dp.SubRegionInfoList.Select((sub, i) =>
                DetectorBinning.GetTimeDelay(
                    sub.PathLength,
                    ops[i].N)
            ).Sum();

            var ia = DetectorBinning.WhichBin(Math.Acos(dp.Direction.Uz), _angle.Count, _angle.Delta, _angle.Start);

            Mean[ia] += dp.Weight;
            SecondMoment[ia] += dp.Weight * dp.Weight;
        }

        public void Normalize(long numPhotons)
        {
            for (int ia = 0; ia < _angle.Count; ia++)
            {
                Mean[ia] /= 2.0 * Math.PI * Math.Sin((ia + 0.5) * _angle.Delta) * _angle.Delta * numPhotons;
            }
        }

        //public double[] Mean 
        //{
        //    get { return _tOfAngle; }
        //}
        //public double[] SecondMoment
        //{
        //    get { return _tOfAngleSecondMoment; }
        //}

    }
}
