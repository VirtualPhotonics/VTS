using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;
using System.Numerics;

namespace Vts.MonteCarlo.TallyActions
{
    /// <summary>
    /// Implements ITerminationTally<double[,]>.  Tally for reflectance as a function 
    /// of Rho and Omega.
    /// </summary>
    public class ROfRhoAndOmegaTally : ITerminationTally<Complex[,]>
    {
        private DoubleRange _rho;
        private DoubleRange _omega;

        public ROfRhoAndOmegaTally(DoubleRange rho, DoubleRange omega)
        {
            _rho = rho;
            _omega = omega;
            Mean = new Complex[_rho.Count - 1, _omega.Count - 1];
            SecondMoment = new Complex[_rho.Count - 1, _omega.Count - 1];
        }

        public Complex[,] Mean { get; set; }
        public Complex[,] SecondMoment { get; set; }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutTop);
        }

        public void Tally(PhotonDataPoint dp, IList<OpticalProperties> ops)
        {
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), _rho.Count - 1, _rho.Delta, _rho.Start);
            var totalTime = dp.SubRegionInfoList.Select((sub, i) =>
                DetectorBinning.GetTimeDelay(
                    sub.PathLength,
                    ops[i].N)
                ).Sum();

            for (int iw = 0; iw < _omega.Count - 1; ++iw)
            {
                double freq = (iw + 1) * _omega.Delta;
                /* convert to Hz-sec from MHz-ns 1e-6*1e9=1e-3 */
                Mean[ir, iw] += dp.Weight * ( Math.Cos(-2 * Math.PI * freq * totalTime * 1e-3) +
                    Complex.ImaginaryOne * Math.Sin(-2 * Math.PI * freq * totalTime * 1e-3) );
                // CKH TODO CHECK: is second moment of complex tally squared or square of real and imag separately?
                SecondMoment[ir, iw] += (dp.Weight * (Math.Cos(-2 * Math.PI * freq * totalTime * 1e-3) +
                    Complex.ImaginaryOne * Math.Sin(-2 * Math.PI * freq * totalTime * 1e-3))) *
                    (dp.Weight * (Math.Cos(-2 * Math.PI * freq * totalTime * 1e-3) +
                    Complex.ImaginaryOne * Math.Sin(-2 * Math.PI * freq * totalTime * 1e-3)));
            }
        }

        public void Normalize(long numPhotons)
        {
            for (int ir = 0; ir < _rho.Count - 1; ir++)
            {
                for (int iw = 0; iw < _omega.Count - 1; iw++)
                {
                    Mean[ir, iw] /= 2.0 * Math.PI * (ir + 0.5) * _rho.Delta * _rho.Delta * numPhotons;
                }
            }
        }
    }
}
