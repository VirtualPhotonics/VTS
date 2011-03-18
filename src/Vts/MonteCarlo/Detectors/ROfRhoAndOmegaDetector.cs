using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;
using System.Numerics;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements ITerminationDetector<double[,]>.  Tally for reflectance as a function 
    /// of Rho and Omega.
    /// This implementation works for Analog, DAW and CAW.
    /// </summary>
    public class ROfRhoAndOmegaDetector : TallyBase, ITerminationDetector<Complex[,]>
    {
        public ROfRhoAndOmegaDetector(DoubleRange rho, DoubleRange omega, ITissue tissue)
            : base(tissue)
        {
            Rho = rho;
            Omega = omega;
            Mean = new Complex[Rho.Count - 1, Omega.Count - 1];
            SecondMoment = new Complex[Rho.Count - 1, Omega.Count - 1];
            TallyType = TallyType.ROfRhoAndOmega;
            TallyCount = 0;
        }
        
        [IgnoreDataMember]
        public Complex[,] Mean { get; set; }

        [IgnoreDataMember]
        public Complex[,] SecondMoment { get; set; }

        public TallyType TallyType { get; set; }

        public long TallyCount { get; set; }

        public DoubleRange Rho { get; set; }

        public DoubleRange Omega { get; set; }

        public void Tally(PhotonDataPoint dp)
        {
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
            var totalTime = dp.TotalTime;
            for (int iw = 0; iw < Omega.Count - 1; ++iw)
            {
                double freq = (iw + 1) * Omega.Delta;
                /* convert to Hz-sec from MHz-ns 1e-6*1e9=1e-3 */
                Mean[ir, iw] += dp.Weight * ( Math.Cos(-2 * Math.PI * freq * totalTime * 1e-3) +
                    Complex.ImaginaryOne * Math.Sin(-2 * Math.PI * freq * totalTime * 1e-3) );
                // CKH TODO CHECK: is second moment of complex tally squared or square of real and imag separately?
                SecondMoment[ir, iw] += (dp.Weight * (Math.Cos(-2 * Math.PI * freq * totalTime * 1e-3) +
                    Complex.ImaginaryOne * Math.Sin(-2 * Math.PI * freq * totalTime * 1e-3))) *
                    (dp.Weight * (Math.Cos(-2 * Math.PI * freq * totalTime * 1e-3) +
                    Complex.ImaginaryOne * Math.Sin(-2 * Math.PI * freq * totalTime * 1e-3)));
            }
            TallyCount++;
        }

        public void Normalize(long numPhotons)
        {
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (int iw = 0; iw < Omega.Count - 1; iw++)
                {
                    Mean[ir, iw] /= 2.0 * Math.PI * (ir + 0.5) * Rho.Delta * Rho.Delta * numPhotons;
                }
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutTop);
        }
    }
}
