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
    /// Implements ITerminationTally&lt;double[]&gt;.  Tally for transmittance as a function 
    /// of Rho.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    [KnownType(typeof(TOfRhoDetector))]
    public class TOfRhoDetector : IDetector<double[]> //ISurfaceDetector<double[]>
    {
        private bool _tallySecondMoment;
        /// <summary>
        /// Returns an instance of TOfRhoDetector
        /// </summary>
        /// <param name="rho"></param>
        public TOfRhoDetector(DoubleRange rho, bool tallySecondMoment, String name)
        {
            Rho = rho;
            _tallySecondMoment = tallySecondMoment;
            Mean = new double[Rho.Count - 1];
            SecondMoment = null;
            if (_tallySecondMoment)
            {
                SecondMoment = new double[Rho.Count - 1];
            }
            TallyType = TallyType.TOfRho;
            Name = name;
            TallyCount = 0;
        }

        /// <summary>
        /// Returns a default instance of TOfRhoDetector (for serialization purposes only)
        /// </summary>
        public TOfRhoDetector()
            : this(new DoubleRange(), true, TallyType.TOfRho.ToString())
        {
        }

        [IgnoreDataMember]
        public double[] Mean { get; set; }

        [IgnoreDataMember]
        public double[] SecondMoment { get; set; }

        public TallyType TallyType { get; set; }

        public String Name { get; set; }

        public long TallyCount { get; set; }

        public DoubleRange Rho { get; set; }

        public void Tally(Photon photon)
        {
            Tally(photon.DP);
        }
        public void Tally(PhotonDataPoint dp)
        {
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);

            Mean[ir] += dp.Weight;
            if (_tallySecondMoment)
            {
                SecondMoment[ir] += dp.Weight * dp.Weight;
            }
            TallyCount++;
        }

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * normalizationFactor;
                Mean[ir] /= areaNorm * numPhotons;
                if (_tallySecondMoment)
                {
                    SecondMoment[ir] /= areaNorm * areaNorm * numPhotons;
                }
            }
        }

        //public bool ContainsPoint(PhotonDataPoint dp)
        //{
        //    return (dp.StateFlag.HasFlag(PhotonStateType.PseudoTransmissionDomainBottomBoundary));
        //}
    }
}
