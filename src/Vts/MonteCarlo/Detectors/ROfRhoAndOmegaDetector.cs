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
    /// <summary>
    /// Implements ISurfaceDetector&lt;double[,]&gt;.  Tally for reflectance as a function 
    /// of Rho and Omega.
    /// This implementation works for Analog, DAW and CAW.
    /// </summary>
    [KnownType(typeof(ROfRhoAndOmegaDetector))]
    public class ROfRhoAndOmegaDetector : ISurfaceDetector<Complex[,]>
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
            SecondMoment = null;
            if (_tallySecondMoment)
            {
                SecondMoment = new Complex[Rho.Count - 1, Omega.Count - 1];
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
                // convert to Hz-sec from GHz-ns 1e-9*1e9=1
                Mean[ir, iw] += dp.Weight * ( Math.Cos(-2 * Math.PI * freq * totalTime) +
                    Complex.ImaginaryOne * Math.Sin(-2 * Math.PI * freq * totalTime) );
                if (_tallySecondMoment)
                {
                    // second moment of complex tally is square of real and imag separately
                    SecondMoment[ir, iw] += 
                        dp.Weight * (Math.Cos(-2 * Math.PI * freq * totalTime)) *
                        dp.Weight * (Math.Cos(-2 * Math.PI * freq * totalTime)) +
                        Complex.ImaginaryOne *
                        dp.Weight * (Math.Sin(-2 * Math.PI * freq * totalTime)) *
                        dp.Weight * (Math.Sin(-2 * Math.PI * freq * totalTime));
                }
            }
            TallyCount++;
        }

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta; 
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (int iw = 0; iw < Omega.Count - 1; iw++)
                {
                    var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * normalizationFactor;
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
            return true; // or, possibly test for NA or confined position, etc
            //return (dp.StateFlag.Has(PhotonStateType.PseudoTransmissionDomainTopBoundary));
        }
    }
}
