using System;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements IDetector&lt;double[,]&gt;.  Tally for transmittance as a function 
    /// of Rho and Angle.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    [KnownType(typeof(TOfRhoAndAngleDetector))]
    public class TOfRhoAndAngleDetector : IDetector<double[,]> 
    {
        private bool _tallySecondMoment;
        /// <summary>
        /// constructor for transmittance as a function of rho and angle detector input
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="angle">angle binning</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment info for error results</param>
        /// <param name="name">detector name</param>
        public TOfRhoAndAngleDetector(DoubleRange rho, DoubleRange angle, bool tallySecondMoment, String name)
        {
            Rho = rho;
            Angle = angle;
            _tallySecondMoment = tallySecondMoment;
            Mean = new double[Rho.Count - 1, Angle.Count - 1];
            SecondMoment = null;
            if (_tallySecondMoment)
            {
                SecondMoment = new double[Rho.Count - 1, Angle.Count - 1];
            }
            TallyType = "TOfRhoAndAngle";
            Name = name;
            TallyCount = 0;
        }

        /// <summary>
        /// Returns a default instance of TOfRhoAndAngleDetector (for serialization purposes only)
        /// </summary>
        public TOfRhoAndAngleDetector()
            : this(new DoubleRange(), new DoubleRange(), true, "TOfRhoAndAngle")
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
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
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
            var ia = DetectorBinning.WhichBin(Math.Acos(photon.DP.Direction.Uz), Angle.Count - 1, Angle.Delta, 0);
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(photon.DP.Position.X, photon.DP.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);

            Mean[ir, ia] += photon.DP.Weight;
            if (_tallySecondMoment)
            {
                SecondMoment[ir, ia] += photon.DP.Weight * photon.DP.Weight;
            }
            TallyCount++;
        }
        /// <summary>
        /// method to normalize detector results after numPhotons launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta * 2.0 * Math.PI * Angle.Delta;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (int ia = 0; ia < Angle.Count - 1; ia++)
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

        //public bool ContainsPoint(PhotonDataPoint dp)
        //{
        //    return (dp.StateFlag.Has(PhotonStateType.PseudoTransmissionDomainBottomBoundary));
        //}

    }
}
