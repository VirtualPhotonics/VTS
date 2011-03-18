using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements IHistoryTally<double[,]>.  Tally for MomentumTransfer(rho,z).
    /// </summary>
    public class MomentumTransferOfRhoAndZDetector : IHistoryDetector<double[,]>
    {
        private ITissue _tissue;

        public MomentumTransferOfRhoAndZDetector(DoubleRange rho, DoubleRange z, ITissue tissue)
        {
            Rho = rho;
            Z = z;
            _tissue = tissue;
            Mean = new double[Rho.Count - 1, Z.Count - 1];
            SecondMoment = new double[Rho.Count - 1, Z.Count - 1];
            TallyType = TallyType.MomentumTransferOfRhoAndZ;
            TallyCount = 0;
        }

        [IgnoreDataMember]
        public double[,] Mean { get; set; }

        [IgnoreDataMember]
        public double[,] SecondMoment { get; set; }

        public TallyType TallyType { get; set; }

        public long TallyCount { get; set; }

        public DoubleRange Rho { get; set; }

        public DoubleRange Z { get; set; }

        public void Tally(PhotonDataPoint previousDP, PhotonDataPoint dp)
        {
            // calculate momentum transfer
            double cosineBetweenTrajectories = Direction.GetDotProduct(previousDP.Direction, dp.Direction);

            var momentumTransfer = 1 - cosineBetweenTrajectories;

            // calculate the radial and time bins to attribute the deposition
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
            var iz = DetectorBinning.WhichBin(dp.Position.Z, Z.Count - 1, Z.Delta, Z.Start);

            Mean[ir, iz] += momentumTransfer;
            SecondMoment[ir, iz] += momentumTransfer * momentumTransfer;
            TallyCount++;
        }

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta * Rho.Delta * Z.Delta * numPhotons;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (int iz = 0; iz < Z.Count - 1; iz++)
                {
                    // need to check that this normalization makes sense for momentum transfer
                    Mean[ir, iz] /= (ir + 0.5) * normalizationFactor;
                }
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true;
        }
    }
}