using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.TallyActions
{
    /// <summary>
    /// Implements IHistoryTally<double[,,]>.  Tally for Fluence(rho,z,t).
    /// Note: this tally currently only works with discrete absorption weighting
    /// </summary>
    public class FluenceOfRhoAndZAndTimeTally : IHistoryTally<double[,,]>
    {
        private DoubleRange _rho;
        private DoubleRange _z;
        private DoubleRange _time;
        private ITissue _tissue;

        public FluenceOfRhoAndZAndTimeTally(DoubleRange rho, DoubleRange z, DoubleRange time, 
            ITissue tissue, AbsorptionWeightingType awt)
        {
            _rho = rho;
            _z = z;
            _time = time;
            _tissue = tissue;
            Mean = new double[_rho.Count - 1, _z.Count - 1, _time.Count - 1];
            SecondMoment = new double[_rho.Count - 1, _z.Count - 1, _time.Count - 1];
            SetAbsorbAction(awt);
        }

        public double[, ,] Mean { get; set; }
        public double[, ,] SecondMoment { get; set; }
        public Action<double, double> AbsorbAction { get; private set; }

        private void SetAbsorbAction(AbsorptionWeightingType awt)
        {
            switch (awt)
            {
                case AbsorptionWeightingType.Analog:
                    AbsorbAction = AbsorbAnalog;
                    break;
                //case AbsorptionWeightingType.Continuous:
                //    AbsorbAction = AbsorbContinuous;
                //    break;
                case AbsorptionWeightingType.Discrete:
                default:
                    AbsorbAction = AbsorbDiscrete;
                    break;
            }
        }
        private double _dw;
        private double _nextDw;
        private PhotonStateType _pst;
        public void Tally(PhotonDataPoint previousDP, PhotonDataPoint dp,
            IList<OpticalProperties> ops)
        {
            // history tallies currently don't carry along subregionInfoList so can't determine
            // total Pathlength from it
            var totalTime = dp.SubRegionInfoList.Select((sub, i) =>
                DetectorBinning.GetTimeDelay(
                    sub.PathLength,
                    ops[i].N)
            ).Sum();

            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), _rho.Count - 1, _rho.Delta, _rho.Start);
            var iz = DetectorBinning.WhichBin(dp.Position.Z, _z.Count - 1, _z.Delta, _z.Start);
            var it = DetectorBinning.WhichBin(totalTime, _time.Count - 1, _time.Delta, _time.Start);
            //double dw = previousDP.Weight * ops[_tissue.GetRegionIndex(dp.Position)].Mua /
            //    (ops[_tissue.GetRegionIndex(dp.Position)].Mua + ops[_tissue.GetRegionIndex(dp.Position)].Mus);
            _pst = dp.StateFlag;
            _dw = previousDP.Weight;
            _nextDw = dp.Weight;
            AbsorbAction(ops[_tissue.GetRegionIndex(dp.Position)].Mua, ops[_tissue.GetRegionIndex(dp.Position)].Mus);
            Mean[ir, iz, it] += _dw / ops[_tissue.GetRegionIndex(dp.Position)].Mua;
            SecondMoment[ir, iz, it] += (_dw / ops[_tissue.GetRegionIndex(dp.Position)].Mua) *
                (_dw / ops[_tissue.GetRegionIndex(dp.Position)].Mua);
        }
        public void AbsorbAnalog(double mua, double mus)
        {
            if (_pst != PhotonStateType.Absorbed)
            {
                _dw = 0.0;
            }
            else
            {
                _dw *= mua / (mua + mus);
            }
        }
        public void AbsorbDiscrete(double mua, double mus)
        {
            if (_dw == _nextDw) // pseudo collision, so no tally
            {
                _dw = 0.0;
            }
            else
            {
                _dw *= mua / (mua + mus);
            }
        }
        public void Normalize(long numPhotons)
        {
            for (int ir = 0; ir < _rho.Count - 1; ir++)
            {
                for (int iz = 0; iz < _z.Count - 1; iz++)
                {
                    for (int it = 0; it < _time.Count - 1; it++)
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

    }
}