using System;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements IDetector&lt;double[]&gt;.  Tally for transmittance as a function 
    /// of Rho.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    [KnownType(typeof(TOfRhoDetector))]
    public class TOfRhoDetector : IDetector<double[]> 
    {
        private bool _tallySecondMoment;
        /// <summary>
        /// constructor for transmittance as a function of rho detector input
        /// </summary>
        /// <param name="rho">rho tally</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment info for error results</param>
        /// <param name="name">detector name</param>
        public TOfRhoDetector(DoubleRange rho, bool tallySecondMoment, String name)
        {
            Rho = rho;
            _tallySecondMoment = tallySecondMoment;
            Mean = new double[Rho.Count - 1];
            SecondMoment = null;
            if (_tallySecondMoment)
            {
                SecondMoment = new double[Rho.Count - 1];
            }
            TallyType = TallyType.TOfRho;
            Name = name;
            TallyCount = 0;
        }

        /// <summary>
        /// Returns a default instance of TOfRhoDetector (for serialization purposes only)
        /// </summary>
        public TOfRhoDetector()
            : this(new DoubleRange(), true, TallyType.TOfRho.ToString())
        {
        }

        /// <summary>
        /// detector mean
        /// </summary>
        [IgnoreDataMember]
        public double[] Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember]
        public double[] SecondMoment { get; set; }

        /// <summary>
        /// detector identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name, default uses TallyType, but can be user-specified
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// number of time detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }
        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(photon.DP.Position.X, photon.DP.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);

            Mean[ir] += photon.DP.Weight;
            if (_tallySecondMoment)
            {
                SecondMoment[ir] += photon.DP.Weight * photon.DP.Weight;
            }
            TallyCount++;
        }

        /// <summary>
        /// method to normalize detector tally results
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * normalizationFactor;
                Mean[ir] /= areaNorm * numPhotons;
                if (_tallySecondMoment)
                {
                    SecondMoment[ir] /= areaNorm * areaNorm * numPhotons;
                }
            }
        }

        //public bool ContainsPoint(PhotonDataPoint dp)
        //{
        //    return (dp.StateFlag.HasFlag(PhotonStateType.PseudoTransmissionDomainBottomBoundary));
        //}
    }
}
