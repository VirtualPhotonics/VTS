using System;
using System.Runtime.Serialization;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements IDetector&lt;double&gt;.  Tally for diffuse transmittance.
    /// This implemenation works for Analog, DAW and CAW processing.
    /// </summary>
    [KnownType(typeof(TDiffuseDetector))]
    public class TDiffuseDetector : IDetectorOld<double> 
    {
        private bool _tallySecondMoment;
        /// <summary>
        /// Returns an instance of TDiffuseDetector
        /// </summary>
        public TDiffuseDetector(bool tallySecondMoment, String name)
        {
            _tallySecondMoment = tallySecondMoment;
            Mean = 0.0;
            SecondMoment = 0.0;
            TallyType = "TDiffuse";
            Name = name;
            TallyCount = 0;
        }
        /// <summary>
        /// Returns a default instance of TDiffuseDetector (for serialization purposes only)
        /// </summary>
        public TDiffuseDetector()
            : this(true, "TDiffuse")
        {
        }
        /// <summary>
        /// mean of detector tally
        /// </summary>
        public double Mean { get; set; }
        /// <summary>
        /// second moment of detector tally
        /// </summary>
        public double SecondMoment { get; set; }

        /// <summary>
        /// detector identifier
        /// </summary>
        public string TallyType { get; set; }
        /// <summary>
        /// detector name, default uses TallyType, but can be user specified
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// number of times detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            Mean += photon.DP.Weight;
            if (_tallySecondMoment)
            {
                SecondMoment += photon.DP.Weight * photon.DP.Weight;
            }
            TallyCount++;
        }

        /// <summary>
        /// method to normalize detector results after all photons launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            Mean /= numPhotons;
            if (_tallySecondMoment)
            {
                SecondMoment /= numPhotons;
            }
        }

        //public bool ContainsPoint(PhotonDataPoint dp)
        //{
        //    return (dp.StateFlag.Has(PhotonStateType.PseudoTransmissionDomainBottomBoundary));
        //}
    }
}
