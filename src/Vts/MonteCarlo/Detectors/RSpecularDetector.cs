using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements IDetector&lt;double&gt;.  Tally for diffuse reflectance.
    /// This implementation works for Analog, DAW and CAW.
    /// </summary>
    [KnownType(typeof(RSpecularDetector))]
    public class RSpecularDetector : IDetector<double> 
    {
        private bool _tallySecondMoment;
        /// <summary>
        /// Returns an instance of SpecularDetector
        /// </summary>
        public RSpecularDetector(bool tallySecondMoment, String name)
        {
            Mean = 0;
            SecondMoment = 0;
            TallyType = TallyType.RSpecular;
            Name = name;
            TallyCount = 0;
            _tallySecondMoment = tallySecondMoment;
        }
        /// <summary>
        /// Returns a default instance of RDiffuseDetector (for serialization purposes only)
        /// </summary>
        public RSpecularDetector()
            : this(true, TallyType.RSpecular.ToString())
        {
        }

        public double Mean { get; set; }

        public double SecondMoment { get; set; }

        public TallyType TallyType { get; set; }

        public String Name { get; set; }

        public long TallyCount { get; set; }

        public void Tally(Photon photon)
        {
            Tally(photon.DP);
        }
        public void Tally(PhotonDataPoint dp)
        {
            Mean += dp.Weight;
            if (_tallySecondMoment)
            {
                SecondMoment += dp.Weight * dp.Weight;
            }
            TallyCount++;
        }

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
