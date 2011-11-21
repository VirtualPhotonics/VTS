using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements IDetector&lt;double[]&gt;.  Tally for Surface Radiance as a function 
    /// of Rho.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    [KnownType(typeof(RadianceOfRhoDetector))]
    public class RadianceOfRhoDetector : IDetector<double[]> 
    {
        private bool _tallySecondMoment;

        /// <summary>
        /// constructor for surface radiance as a function of rho detector input
        /// </summary>
        /// <param name="zDepth">z constant defining surface of tally</param>
        /// <param name="rho">rho binning</param>
        /// <param name="tissue">tissue definition</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment info for error results</param>
        /// <param name="name">detector name</param>
        public RadianceOfRhoDetector(
            double zDepth, 
            DoubleRange rho, 
            ITissue tissue,
            bool tallySecondMoment, 
            String name)
        {
            Rho = rho;
            ZDepth = zDepth;
            _tallySecondMoment = tallySecondMoment;
            Mean = new double[Rho.Count - 1];
            SecondMoment = null;
            if (_tallySecondMoment)
            {
                SecondMoment = new double[Rho.Count - 1];
            }
            TallyType = TallyType.RadianceOfRho;
            Name = name;
            TallyCount = 0;
        }

        /// <summary>
        ///  Returns a default instance of RadianceOfRhoDetector (for serialization purposes only)
        /// </summary>
        public RadianceOfRhoDetector()
            : this(10.0, new DoubleRange(), new MultiLayerTissue(), true, TallyType.RadianceOfRho.ToString())
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
        /// detector name, default uses TallyType, but can be user specified
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// number of times detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }
        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// z constant defining surface of tally
        /// </summary>
        public double ZDepth { get; set; }

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(photon.DP.Position.X, photon.DP.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
                
            // update tally
            Mean[ir] += photon.DP.Weight / photon.DP.Direction.Uz;
            if (_tallySecondMoment)
            {
                SecondMoment[ir] += (photon.DP.Weight / photon.DP.Direction.Uz) * (photon.DP.Weight / photon.DP.Direction.Uz);
            }
            TallyCount++;
        }
         
        /// <summary>
        /// method to normalize detector results after numPhotons launched
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
        /// <summary>
        /// method to determine whether photon within detector
        /// </summary>
        /// <param name="dp"></param>
        /// <returns></returns>
        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true; // or, possibly test for NA or confined position, etc
            //return (dp.StateFlag.Has(PhotonStateType.PseudoTransmissionDomainTopBoundary));
        }

    }
}
