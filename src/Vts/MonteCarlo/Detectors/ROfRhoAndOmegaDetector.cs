using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;
using System.Numerics;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Detectors
{
    [KnownType(typeof(ROfRhoAndOmegaDetector))]
    /// <summary>
    /// Implements ITerminationDetector<double[,]>.  Tally for reflectance as a function 
    /// of Rho and Omega.
    /// This implementation works for Analog, DAW and CAW.
    /// </summary>
    public class ROfRhoAndOmegaDetector : ITerminationDetector<Complex[,]>
    {
        private bool _tallySecondMoment;
        /// <summary>
        /// Returns an instance of ROfRhoAndAngleDetector
        /// </summary>
        /// <param name="rho"></param>
        /// <param name="omega"></param>
        /// <param name="tissue"></param>
        public ROfRhoAndOmegaDetector(DoubleRange rho, DoubleRange omega, bool tallySecondMoment, String name)
        {
            Rho = rho;
            Omega = omega;
            _tallySecondMoment = tallySecondMoment;
            Mean = new Complex[Rho.Count - 1, Omega.Count - 1];
            if (_tallySecondMoment)
            {
                SecondMoment = new Complex[Rho.Count - 1, Omega.Count - 1];
            }
            else
            {
                SecondMoment = null;
            }
            TallyType = TallyType.ROfRhoAndOmega;
            Name = name;
            TallyCount = 0;
        }
        /// <summary>
        /// Returns a default instance of ROfRhoAndAngleDetector (for serialization purposes only)
        /// </summary>
        public ROfRhoAndOmegaDetector()
            : this(new DoubleRange(), new DoubleRange(), true, TallyType.ROfRhoAndOmega.ToString())
        {
            
        }
        
        [IgnoreDataMember]
        public Complex[,] Mean { get; set; }

        [IgnoreDataMember]
        public Complex[,] SecondMoment { get; set; }

        public TallyType TallyType { get; set; }

        public String Name { get; set; }

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
                if (_tallySecondMoment)
                {
                    // CKH TODO CHECK: is second moment of complex tally squared or square of real and imag separately?
                    SecondMoment[ir, iw] += (dp.Weight * (Math.Cos(-2 * Math.PI * freq * totalTime * 1e-3) +
                        Complex.ImaginaryOne * Math.Sin(-2 * Math.PI * freq * totalTime * 1e-3))) *
                        (dp.Weight * (Math.Cos(-2 * Math.PI * freq * totalTime * 1e-3) +
                        Complex.ImaginaryOne * Math.Sin(-2 * Math.PI * freq * totalTime * 1e-3)));
                }
            }
            TallyCount++;
        }

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta * Rho.Delta; 
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (int iw = 0; iw < Omega.Count - 1; iw++)
                {
                    var areaNorm = (ir + 0.5) * normalizationFactor;
                    Mean[ir, iw] /=  areaNorm * numPhotons;
                    if (_tallySecondMoment)
                    {
                        SecondMoment[ir, iw] /= areaNorm * areaNorm * numPhotons;
                    }
                }
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutTop);
        }
    }
}
