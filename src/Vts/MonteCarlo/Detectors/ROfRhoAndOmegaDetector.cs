using System;
using System.Linq;
using MathNet.Numerics;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements IDetector&lt;Complex[,]&gt;.  Tally for reflectance as a function 
    /// of Rho and Omega.
    /// This implementation works for Analog, DAW and CAW.
    /// </summary>
    [KnownType(typeof(ROfRhoAndOmegaDetector))]
    public class ROfRhoAndOmegaDetector : IDetector<Complex[,]> 
    {
        private bool _tallySecondMoment;
        private double[] _omegaArray;

        /// <summary>
        /// Returns an instance of ROfRhoAndAngleDetector
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="omega">temporal frequency sampling points (not binned)</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment or not</param>
        /// <param name="name">detector name</param>
        public ROfRhoAndOmegaDetector(DoubleRange rho, DoubleRange omega, bool tallySecondMoment, String name)
        {
            Rho = rho;
            Omega = omega;
            _omegaArray = omega.AsEnumerable().ToArray();
            _tallySecondMoment = tallySecondMoment;
            Mean = new Complex[Rho.Count - 1, Omega.Count];
            SecondMoment = null;
            if (_tallySecondMoment)
            {
                SecondMoment = new Complex[Rho.Count - 1, Omega.Count];
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
        /// <summary>
        /// detector mean
        /// </summary>
        [IgnoreDataMember]
        public Complex[,] Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember]
        public Complex[,] SecondMoment { get; set; }
        /// <summary>
        /// detector identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name, default uses TallyType, but can be user specified
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// number time detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }
        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// temporal frequency sampling points (not binned)
        /// </summary>
        public DoubleRange Omega { get; set; }

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(photon.DP.Position.X, photon.DP.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
            var totalTime = photon.DP.TotalTime;
            for (int iw = 0; iw < Omega.Count; ++iw)
            {
                double freq = _omegaArray[iw];
                // convert to Hz-sec from GHz-ns 1e-9*1e9=1
                Mean[ir, iw] += photon.DP.Weight * ( Math.Cos(-2 * Math.PI * freq * totalTime) +
                    Complex.ImaginaryOne * Math.Sin(-2 * Math.PI * freq * totalTime) );
                if (_tallySecondMoment)
                {
                    // second moment of complex tally is square of real and imag separately
                    SecondMoment[ir, iw] += 
                        photon.DP.Weight * (Math.Cos(-2 * Math.PI * freq * totalTime)) *
                        photon.DP.Weight * (Math.Cos(-2 * Math.PI * freq * totalTime)) +
                        Complex.ImaginaryOne *
                        photon.DP.Weight * (Math.Sin(-2 * Math.PI * freq * totalTime)) *
                        photon.DP.Weight * (Math.Sin(-2 * Math.PI * freq * totalTime));
                }
            }
            TallyCount++;
        }

        /// <summary>
        /// method to normalize detector after numPhotons launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta; 
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (int iw = 0; iw < Omega.Count; iw++)
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

        /// <summary>
        /// Method to determine if photon is within detector
        /// </summary>
        /// <param name="dp">photon data point</param>
        /// <returns>method always returns true</returns>
        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true; // or, possibly test for NA or confined position, etc
            //return (dp.StateFlag.Has(PhotonStateType.PseudoTransmissionDomainTopBoundary));
        }
    }
}
