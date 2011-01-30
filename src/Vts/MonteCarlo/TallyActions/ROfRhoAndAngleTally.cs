using System;
using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.TallyActions
{
    /// <summary>
    /// Implements ITerminationTally<double[,]>.  Tally for reflectance as a function 
    /// of Rho and Angle.
    /// </summary>
    public class ROfRhoAndAngleTally : ITerminationTally<double[,]>
    {
        private DoubleRange _rho;
        private DoubleRange _angle;
        //private double[,] _rOfRhoAndAngle;
        //private double[,] _rOfRhoAndAngleSecondMoment;

        public ROfRhoAndAngleTally(DoubleRange rho, DoubleRange angle)
        {
            _rho = rho;
            _angle = angle;
            Mean = new double[_rho.Count - 1, _angle.Count];
            SecondMoment = new double[_rho.Count - 1, _angle.Count];
        }

        public double[,] Mean { get; set; }
        public double[,] SecondMoment { get; set; }
        
        //public double[,] Mean 
        //{
        //    get { return _rOfRhoAndAngle; }
        //}

        //public double[,] SecondMoment
        //{
        //    get { return _rOfRhoAndAngleSecondMoment; }
        //}


        #region ITally Members

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutTop);
        }
 
        public virtual void Tally(PhotonDataPoint dp, IList<OpticalProperties> ops)
        {
            var ia = DetectorBinning.WhichBin(Math.Acos(dp.Direction.Uz), _angle.Count, _angle.Delta, _angle.Start);
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), _rho.Count - 1, _rho.Delta, _rho.Start);

            Mean[ir, ia] += dp.Weight;
            SecondMoment[ir, ia] += dp.Weight * dp.Weight;
        }

        public void Normalize(long numPhotons)
        {
            for (int ir = 0; ir < _rho.Count - 1; ir++)
            {
                for (int ia = 0; ia < _angle.Count; ia++)
                {
                    Mean[ir, ia] /=
                        2.0 * Math.PI * _rho.Delta * _rho.Delta * 2.0 * Math.PI * _angle.Delta *
                        (ir + 0.5) * Math.Sin((ia + 0.5) * _angle.Delta) * numPhotons;
                }
            }
        }

        #endregion
    }
}
