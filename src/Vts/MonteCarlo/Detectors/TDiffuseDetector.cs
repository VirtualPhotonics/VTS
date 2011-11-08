using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements IDetector&lt;double&gt;.  Tally for diffuse transmittance.
    /// This implemenation works for Analog, DAW and CAW processing.
    /// </summary>
    [KnownType(typeof(TDiffuseDetector))]
    public class TDiffuseDetector : IDetector<double> 
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
            TallyType = TallyType.TDiffuse;
            Name = name;
            TallyCount = 0;
        }
        /// <summary>
        /// Returns a default instaf TDiffuseDetector (for serialization purposes only)
        /// </summary>
        public TDiffuseDetector()
            : this(true, TallyType.TDiffuse.ToString())
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
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name, default uses TallyType, but can be user specified
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// number of times detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }

        public void Tally(Photon photon)
        {
            Tally(photon.DP);
        }
        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="dp"></param>
        public void Tally(PhotonDataPoint dp)
        {
            Mean += dp.Weight;
            if (_tallySecondMoment)
            {
                SecondMoment += dp.Weight * dp.Weight;
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
