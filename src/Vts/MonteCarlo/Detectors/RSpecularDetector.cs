using System;
using System.Runtime.Serialization;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements IDetector&lt;double&gt;.  Tally for diffuse reflectance.
    /// This implementation works for Analog, DAW and CAW.
    /// </summary>
    [KnownType(typeof(RSpecularDetector))]
    public class RSpecularDetector : IDetectorOld<double> 
    {
        private bool _tallySecondMoment;
        /// <summary>
        /// Returns an instance of SpecularDetector
        /// </summary>
        public RSpecularDetector(bool tallySecondMoment, String name)
        {
            Mean = 0;
            SecondMoment = 0;
            TallyType = "RSpecular";
            Name = name;
            TallyCount = 0;
            _tallySecondMoment = tallySecondMoment;
        }
        /// <summary>
        /// Returns a default instance of RDiffuseDetector (for serialization purposes only)
        /// </summary>
        public RSpecularDetector()
            : this(true, "RSpecular")
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
        /// number of time detector gets tallied to
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
        /// method to normalize detector results after numPhotons launched
        /// </summary>
        /// <param name="numPhotons">number of launched photons</param>
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
        //    return (dp.StateFlag.Has(PhotonStateType.PseudoReflectionDomainTopBoundary));
        //}
    }
}
