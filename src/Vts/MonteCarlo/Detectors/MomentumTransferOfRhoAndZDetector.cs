using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements IHistoryTally&lt;double[,]&gt;.  Tally for MomentumTransfer(rho,z).
    /// </summary>
    [KnownType(typeof(MomentumTransferOfRhoAndZDetector))]
    public class MomentumTransferOfRhoAndZDetector : IDetector<double[,]> //IVolumeDetector<double[,]>
    {
        private bool _tallySecondMoment;

        /// <summary>
        /// Returns an instance of MomentumTransferOfRhoAndZDetector
        /// </summary>
        /// <param name="rho"></param>
        /// <param name="z"></param>
        public MomentumTransferOfRhoAndZDetector(
            DoubleRange rho, 
            DoubleRange z, 
            bool tallySecondMoment,
            String name)
        {
            Rho = rho;
            Z = z;
            Mean = new double[Rho.Count - 1, Z.Count - 1];
            if (_tallySecondMoment)
            {
                SecondMoment = new double[Rho.Count - 1, Z.Count - 1];
            }
            TallyType = TallyType.MomentumTransferOfRhoAndZ;
            _tallySecondMoment = tallySecondMoment;
            Name = name;
            TallyCount = 0;
        }

        /// <summary>
        /// Returns a default instance of MomentumTransferOfRhoAndZDetector (for serialization purposes only)
        /// </summary>
        public MomentumTransferOfRhoAndZDetector()
            : this(
            new DoubleRange(), 
            new DoubleRange(), 
            true, // tally SecondMoment
            TallyType.MomentumTransferOfRhoAndZ.ToString())
        {
        }

        [IgnoreDataMember]
        public double[,] Mean { get; set; }

        [IgnoreDataMember]
        public double[,] SecondMoment { get; set; }

        public TallyType TallyType { get; set; }

        public String Name { get; set; }

        public long TallyCount { get; set; }

        public DoubleRange Rho { get; set; }

        public DoubleRange Z { get; set; }

        public void Tally(Photon photon)
        {
            // todo: is this logically consistent at any place that could call Tally(photon)?
            Tally(photon.History.PreviousDP, photon.History.CurrentDP);
        }

        // collision tally
        public void Tally(PhotonDataPoint previousDP, PhotonDataPoint dp)
        {
            // calculate momentum transfer
            double cosineBetweenTrajectories = Direction.GetDotProduct(previousDP.Direction, dp.Direction);

            var momentumTransfer = 1 - cosineBetweenTrajectories;

            // calculate the radial and time bins to attribute the deposition
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
            var iz = DetectorBinning.WhichBin(dp.Position.Z, Z.Count - 1, Z.Delta, Z.Start);

            Mean[ir, iz] += momentumTransfer;
            if (_tallySecondMoment)
            {
                SecondMoment[ir, iz] += momentumTransfer * momentumTransfer;
            }
            TallyCount++;
        }

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta * Z.Delta;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (int iz = 0; iz < Z.Count - 1; iz++)
                {
                    var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * normalizationFactor;
                    // need to check that this normalization makes sense for momentum transfer
                    Mean[ir, iz] /=  areaNorm * numPhotons;
                    if (_tallySecondMoment)
                    {
                        SecondMoment[ir, iz] /= areaNorm * areaNorm * numPhotons;
                    }
                }
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true;
        }
    }
}