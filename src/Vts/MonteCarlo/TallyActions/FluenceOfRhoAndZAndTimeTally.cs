using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.TallyActions
{
    /// <summary>
    /// Implements ITally<double[,,]>.  Tally for Fluence(rho,z,t).
    /// Note: this tally currently only works with discrete absorption weighting
    /// </summary>
    public class FluenceOfRhoAndZAndTimeTally : IHistoryTally<double[,,]>
    {
        private DoubleRange _rho;
        private DoubleRange _z;
        private DoubleRange _time;
        private ITissue _tissue;

        public FluenceOfRhoAndZAndTimeTally(DoubleRange rho, DoubleRange z, DoubleRange time, ITissue tissue)
        {
            _rho = rho;
            _z = z;
            _time = time;
            _tissue = tissue;
            Mean = new double[_rho.Count, _z.Count, _time.Count];
            SecondMoment = new double[_rho.Count, _z.Count, _time.Count];
        }
        //static PhotonDataPoint _previousDP;
        //static bool _firstPoint = true;
        public void Tally(PhotonDataPoint previousDP, PhotonDataPoint dp,
            IList<OpticalProperties> ops)
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
                var totalTime = dp.SubRegionInfoList.Select((sub, i) =>
                    DetectorBinning.GetTimeDelay(
                        sub.PathLength,
                        ops[i].N)
                ).Sum();

                var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), _rho.Count, _rho.Delta, _rho.Start);
                var iz = DetectorBinning.WhichBin(dp.Position.Z, _z.Count, _z.Delta, _z.Start);
                var it = DetectorBinning.WhichBin(totalTime, _time.Count, _time.Delta, _time.Start);
                double dw = previousDP.Weight * ops[_tissue.GetRegionIndex(dp.Position)].Mua /
                    (ops[_tissue.GetRegionIndex(dp.Position)].Mua + ops[_tissue.GetRegionIndex(dp.Position)].Mus);
                Mean[ir, iz, it] += dw / ops[_tissue.GetRegionIndex(dp.Position)].Mua;
                SecondMoment[ir, iz, it] += (dw / ops[_tissue.GetRegionIndex(dp.Position)].Mua) *
                    (dw / ops[_tissue.GetRegionIndex(dp.Position)].Mua);
            //}
            //_previousDP = dp;
            // if last photon in history, reset _firstPoint flag
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
                    for (int it = 0; it < _time.Count; it++)
                    {
                        Mean[ir, iz, it] /=
                            2.0 * Math.PI * (ir + 0.5) * _rho.Delta * _rho.Delta * _z.Delta * _time.Delta * numPhotons;
                    }
                }
            }
        }
        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true;
        }
        public double[,,] Mean { get; set; }
        public double[,,] SecondMoment { get; set; }

    }
}