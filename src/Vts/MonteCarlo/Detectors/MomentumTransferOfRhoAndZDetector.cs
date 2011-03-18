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
        private DoubleRange _rho;
        private DoubleRange _z;
        private ITissue _tissue;

        public MomentumTransferOfRhoAndZDetector(DoubleRange rho, DoubleRange z, ITissue tissue)
        {
            _rho = rho;
            _z = z;
            _tissue = tissue;
            Mean = new double[_rho.Count - 1, _z.Count - 1];
            SecondMoment = new double[_rho.Count - 1, _z.Count - 1];
            TallyType = TallyType.MomentumTransferOfRhoAndZ;
        }

        [IgnoreDataMember]
        public double[,] Mean { get; set; }

        [IgnoreDataMember]
        public double[,] SecondMoment { get; set; }

        public TallyType TallyType { get; set; }

        public void Tally(PhotonDataPoint previousDP, PhotonDataPoint dp)
        {
            // calculate momentum transfer
            double cosineBetweenTrajectories = Direction.GetDotProduct(previousDP.Direction, dp.Direction);

            var momentumTransfer = 1 - cosineBetweenTrajectories;

            // calculate the radial and time bins to attribute the deposition
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), _rho.Count - 1, _rho.Delta, _rho.Start);
            var iz = DetectorBinning.WhichBin(dp.Position.Z, _z.Count - 1, _z.Delta, _z.Start);

            Mean[ir, iz] += momentumTransfer;
            SecondMoment[ir, iz] += momentumTransfer * momentumTransfer;
        }

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * _rho.Delta * _rho.Delta * _z.Delta * numPhotons;
            for (int ir = 0; ir < _rho.Count - 1; ir++)
            {
                for (int iz = 0; iz < _z.Count - 1; iz++)
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