using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements ISurfaceDetector&lt;double&gt;.  Tally for diffuse reflectance.
    /// This implementation works for Analog, DAW and CAW.
    /// </summary>
    [KnownType(typeof(RDiffuseDetector))]
    public class RDiffuseDetector : ISurfaceDetector<double>
    {
        private bool _tallySecondMoment;

        /// <summary>
        /// Returns an instance of RDiffuseDetector
        /// </summary>
        public RDiffuseDetector(bool tallySecondMoment, String name)
        {
            Mean = 0;
            SecondMoment = 0;
            TallyType = TallyType.RDiffuse;
            Name = name;
            TallyCount = 0;
            _tallySecondMoment = tallySecondMoment;
        }
        /// <summary>
        /// Returns a default instance of RDiffuseDetector (for serialization purposes only)
        /// </summary>
        public RDiffuseDetector()
            : this(true, TallyType.RDiffuse.ToString())
        {
        }
        /// <summary>
        /// detector mean
        /// </summary>
        public double Mean { get; set; }
        /// <summary>
        /// detector second moment
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

 
        /// <summary>
        /// method to tally to detector.  Works for analog, DAW and CAW.  Weight is final surface exiting weight.
        /// </summary>
        /// <param name="dp">photon data point</param>
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
        /// method to normalize detector results after numPhotons are launched
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

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true; // or, possibly test for NA or confined position, etc
        }
    }
}
