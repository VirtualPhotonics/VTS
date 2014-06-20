using System;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements IDetector&lt;double[]&gt;.  Tally for transmittance as a function 
    /// of Angle. 
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class TOfAngleDetector : IDetector<double[]> 
    {
        private bool _tallySecondMoment;
        /// <summary>
        /// constructor for transmittance as a function of angle detector input
        /// </summary>
        /// <param name="angle">angle binning</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment info for error results</param>
        /// <param name="name">detector name</param>
        public TOfAngleDetector(DoubleRange angle, bool tallySecondMoment, String name)
        {
            Angle = angle;
            _tallySecondMoment = tallySecondMoment;
            Mean = new double[Angle.Count];
            SecondMoment = null;
            if (_tallySecondMoment)
            {
                SecondMoment = new double[Angle.Count];
            }
            TallyType = TallyType.TOfAngle;
            Name = name;
            TallyCount = 0;
        }

        /// <summary>
        /// Returns a default instance of TDiffuseDetector (for serialization purposes only)
        /// </summary>
        public TOfAngleDetector()
            : this(new DoubleRange(), true, TallyType.TOfAngle.ToString())
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
        /// detector tally identifier
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

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            // if exiting bottom top surface, Uz > 0 => Acos in [0, pi/2]
            var ia = DetectorBinning.WhichBin(Math.Acos(photon.DP.Direction.Uz), Angle.Count - 1, Angle.Delta, Angle.Start);

            Mean[ia] += photon.DP.Weight;
            if (_tallySecondMoment)
            {
                SecondMoment[ia] += photon.DP.Weight * photon.DP.Weight;
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
            for (int ia = 0; ia < Angle.Count; ia++)
            {
                var areaNorm = Math.Sin((ia + 0.5) * Angle.Delta) * normalizationFactor;
                Mean[ia] /=  areaNorm * numPhotons;
                if (_tallySecondMoment)
                {
                    SecondMoment[ia] /= areaNorm * areaNorm * numPhotons;
                }
            }
        }

        //public bool ContainsPoint(PhotonDataPoint dp)
        //{
        //    return (dp.StateFlag.Has(PhotonStateType.PseudoTransmissionDomainBottomBoundary));
        //}
    }
}
