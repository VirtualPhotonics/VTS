using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.TallyActions
{
    /// <summary>
    /// Implements ITerminationTally<double[,]>.  Tally for transmittance as a function 
    /// of Rho and Angle.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class TOfRhoAndAngleTally : ITerminationTally<double[,]>
    {
        private DoubleRange _rho;
        private DoubleRange _angle;

        public TOfRhoAndAngleTally(DoubleRange rho, DoubleRange angle)
        {
            _rho = rho;
            _angle = angle;
            Mean = new double[_rho.Count - 1, _angle.Count - 1];
            SecondMoment = new double[_rho.Count - 1, _angle.Count - 1];
        }

        public double[,] Mean { get; set; }
        public double[,] SecondMoment { get; set; }

        public void Tally(PhotonDataPoint dp)
        {
            var ia = DetectorBinning.WhichBin(Math.Acos(dp.Direction.Uz), _angle.Count - 1, _angle.Delta, 0);
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), _rho.Count - 1, _rho.Delta, _rho.Start);

            Mean[ir, ia] += dp.Weight;
            SecondMoment[ir, ia] += dp.Weight * dp.Weight;
        }

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = numPhotons * 2.0 * Math.PI * _rho.Delta * _rho.Delta * 2.0 * Math.PI * _angle.Delta;
            for (int ir = 0; ir < _rho.Count - 1; ir++)
            {
                for (int ia = 0; ia < _angle.Count - 1; ia++)
                {
                    Mean[ir, ia] /= (ir + 0.5) * Math.Sin((ia + 0.5) * _angle.Delta) * normalizationFactor;
                }
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutBottom);
        }

    }
}
