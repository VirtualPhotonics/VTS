using System;
using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.TallyActions
{
    /// <summary>
    /// Implements IHistoryTally<double[,]>.  Tally for Fluence(rho,z).
    /// Note: this tally currently only works with discrete absorption weighting
    /// </summary>
    public class FluenceOfRhoAndZTally : IHistoryTally<double[,]>
    {
        private DoubleRange _rho;
        private DoubleRange _z;
        private ITissue _tissue;

        public FluenceOfRhoAndZTally(DoubleRange rho, DoubleRange z, ITissue tissue)
        {
            _rho = rho;
            _z = z;
            _tissue = tissue;
            Mean = new double[_rho.Count, _z.Count];
            SecondMoment = new double[_rho.Count, _z.Count];
        }
        //static PhotonDataPoint _previousDP;
        //static bool _firstPoint = true;
        public void Tally(PhotonDataPoint previousDP, PhotonDataPoint dp, IList<OpticalProperties> ops)
        {
            //if (_firstPoint)
            //{
            //    _firstPoint = false;
            //    _previousDP = new PhotonDataPoint(
            //        dp.Position,
            //        dp.Direction,
            //        dp.Weight,
            //        dp.StateFlag,
            //        dp.SubRegionInfoList
            //        );
            //}
            //else
            //{
                var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), _rho.Count, _rho.Delta, _rho.Start);
                var iz = DetectorBinning.WhichBin(dp.Position.Z, _z.Count, _z.Delta, _z.Start);
                double dw = previousDP.Weight * ops[_tissue.GetRegionIndex(dp.Position)].Mua / 
                    (ops[_tissue.GetRegionIndex(dp.Position)].Mua + ops[_tissue.GetRegionIndex(dp.Position)].Mus);
                Mean[ir, iz] += dw / ops[_tissue.GetRegionIndex(dp.Position)].Mua; 
                SecondMoment[ir, iz] += (dw / ops[_tissue.GetRegionIndex(dp.Position)].Mua) * 
                    (dw / ops[_tissue.GetRegionIndex(dp.Position)].Mua);
            //}
            //_previousDP = dp;
            //// if last photon in history, reset _firstPoint flag
            //if (dp.StateFlag != PhotonStateType.NotSet)
            //{
            //    _firstPoint = true;
            //}
        }

        public void Normalize(long numPhotons)
        {
            for (int ir = 0; ir < _rho.Count; ir++)
            {
                for (int iz = 0; iz < _z.Count; iz++)
                {
                    Mean[ir, iz] /=
                        2.0 * Math.PI * (ir + 0.5) * _rho.Delta * _rho.Delta * _z.Delta * numPhotons;
                }
            }
        }
        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true;
        }
        public double[,] Mean { get; set; }
        public double[,] SecondMoment { get; set; }

    }
}