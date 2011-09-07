using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements ITerminationTally&lt;double&gt;.  Tally for diffuse transmittance.
    /// This implemenation works for Analog, DAW and CAW processing.
    /// </summary>
    [KnownType(typeof(TDiffuseDetector))]
    public class TDiffuseDetector : ISurfaceDetector<double>
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
        //    return (dp.StateFlag.Has(PhotonStateType.PseudoTransmissionDomainBottomBoundary));
        //}
    }
}
