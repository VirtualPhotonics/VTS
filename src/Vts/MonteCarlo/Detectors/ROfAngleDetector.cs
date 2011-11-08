using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements IDetector&lt;double[]&gt;.  Tally for reflectance as a function 
    /// of Angle.
    /// This works for Analog, DAW and CAW.
    /// </summary>
    [KnownType(typeof(ROfAngleDetector))]
    public class ROfAngleDetector : IDetector<double[]> 
    {
        private bool _tallySecondMoment;
        /// <summary>
        /// Returns an instance of RDiffuseDetector
        /// </summary>
        public ROfAngleDetector(DoubleRange angle, bool tallySecondMoment, String name)
        {
            Angle = angle;
            Mean = new double[Angle.Count - 1];
            _tallySecondMoment = tallySecondMoment;
            SecondMoment = null;
            if (_tallySecondMoment)
            {
                SecondMoment = new double[Angle.Count - 1];
            }
            TallyType = TallyType.ROfAngle;
            Name = name;
            TallyCount = 0;
        }
        
        /// <summary>
        /// Returns a default instance of ROfAngleDetector (for serialization purposes only)
        /// </summary>
        public ROfAngleDetector()
            : this(new DoubleRange(), true, TallyType.ROfAngle.ToString())
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
        /// angle binning
        /// </summary>
        public DoubleRange Angle { get; set; }

        public void Tally(Photon photon)
        {
            Tally(photon.DP);
        }
        public void Tally(PhotonDataPoint dp)
        {
            // if exiting tissue top surface, Uz < 0 => Acos in [pi/2, pi]
            var ia = DetectorBinning.WhichBin(Math.Acos(dp.Direction.Uz), Angle.Count - 1, Angle.Delta, Angle.Start);

            Mean[ia] += dp.Weight;
            if (_tallySecondMoment)
            {
                SecondMoment[ia] += dp.Weight * dp.Weight;
            }
            TallyCount++;
        }
        /// <summary>
        /// method to normalize detector results after numPhotons launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Angle.Delta;
            for (int ia = 0; ia < Angle.Count - 1; ia++)
            {
                var areaNorm = Math.Sin((ia + 0.5) * Angle.Delta) * normalizationFactor;
                Mean[ia] /= areaNorm * numPhotons;
                if (_tallySecondMoment)
                {
                    SecondMoment[ia] /= areaNorm * areaNorm * numPhotons;
                }
            }
        }
        /// <summary>
        /// method to determine if photon within detector
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
