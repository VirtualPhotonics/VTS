using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements IDetector&lt;double[,]&gt;.  Tally for reflectance as a function 
    /// of Rho and Angle.
    /// This works for Analog, DAW and CAW processing.
    /// </summary>
    [KnownType(typeof(ROfRhoAndAngleDetector))]
    public class ROfRhoAndAngleDetector : IDetector<double[,]> 
    {
        private bool _tallySecondMoment;
        /// <summary>
        /// constructor for reflectance as a function of rho and angle detector input
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="angle">angle binning</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment info for error results</param>
        /// <param name="name">detector name</param>
        public ROfRhoAndAngleDetector(DoubleRange rho, DoubleRange angle, bool tallySecondMoment, String name)
        {
            Rho = rho;
            Angle = angle;
            _tallySecondMoment = tallySecondMoment;
            Mean = new double[Rho.Count - 1, Angle.Count];
            SecondMoment = null;
            if (_tallySecondMoment)
            {
                SecondMoment = new double[Rho.Count - 1, Angle.Count];
            }
            TallyType = TallyType.ROfRhoAndAngle;
            Name = name;
            TallyCount = 0;
        }

        /// <summary>
        /// Returns a default instance of ROfRhoAndAngleDetector (for serialization purposes only)
        /// </summary>
        public ROfRhoAndAngleDetector()
            : this(new DoubleRange(), new DoubleRange(), true, TallyType.ROfRhoAndAngle.ToString())
        {
        }
        /// <summary>
        /// detector mean
        /// </summary>
        [IgnoreDataMember]
        public double[,] Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember]
        public double[,] SecondMoment { get; set; }

        /// <summary>
        /// detector identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// number of times detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }
        /// <summary>
        /// detector name, default uses TallyType, but can be user specified
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// angle binning
        /// </summary>
        public DoubleRange Angle { get; set; }

        public void Tally(Photon photon)
        {
            Tally(photon.DP);
        }
        public virtual void Tally(PhotonDataPoint dp)
        {
            // if exiting tissue top surface, Uz < 0 => Acos in [pi/2, pi]
            var ia = DetectorBinning.WhichBin(Math.Acos(dp.Direction.Uz), Angle.Count - 1, Angle.Delta, Angle.Start);
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);

            Mean[ir, ia] += dp.Weight;
            if (_tallySecondMoment)
            {
                SecondMoment[ir, ia] += dp.Weight * dp.Weight;
            }
            TallyCount++;
        }
        /// <summary>
        /// method to normalize detector results after all photons launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta * 2.0 * Math.PI * Angle.Delta;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (int ia = 0; ia < Angle.Count; ia++)
                {
                    var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * Math.Sin((ia + 0.5) * Angle.Delta) * normalizationFactor;
                    Mean[ir, ia] /= areaNorm * numPhotons;
                    if (_tallySecondMoment)
                    {
                        SecondMoment[ir, ia] /= areaNorm * areaNorm * numPhotons;
                    }
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
