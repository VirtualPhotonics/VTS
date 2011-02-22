using System;
using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.TallyActions
{
    /// <summary>
    /// Implements IHistoryTally<double[,]>.  Tally for MomentumTransfer(rho,z).
    /// </summary>
    public class MomentumTransferOfRhoAndZTally : IHistoryTally<double[,]>
    {
        private DoubleRange _rho;
        private DoubleRange _z;
        private ITissue _tissue;

        public MomentumTransferOfRhoAndZTally(DoubleRange rho, DoubleRange z, ITissue tissue)
        {
            _rho = rho;
            _z = z;
            _tissue = tissue;
            Mean = new double[_rho.Count - 1, _z.Count - 1];
            SecondMoment = new double[_rho.Count - 1, _z.Count - 1];
        }

        public double[,] Mean { get; set; }
        public double[,] SecondMoment { get; set; }

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
            for (int ir = 0; ir < _rho.Count - 1; ir++)
            {
                for (int iz = 0; iz < _z.Count - 1; iz++)
                {
                    // need to check that this normalization makes sense for momentum transfer
                    Mean[ir, iz] /=
                        2.0 * Math.PI * (ir + 0.5) * _rho.Delta * _rho.Delta * _z.Delta * numPhotons;
                }
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true;
        }
    }
}