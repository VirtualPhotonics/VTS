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

        public FluenceOfRhoAndZTally(DoubleRange rho, DoubleRange z, ITissue tissue, AbsorptionWeightingType awt)
        {
            _rho = rho;
            _z = z;
            _tissue = tissue;
            Mean = new double[_rho.Count - 1, _z.Count - 1];
            SecondMoment = new double[_rho.Count - 1, _z.Count - 1];
            SetAbsorbAction(awt);
        }
        // should following code be put in common class?
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
        public void Tally(PhotonDataPoint previousDP, PhotonDataPoint dp, IList<OpticalProperties> ops)
        {
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), _rho.Count - 1, _rho.Delta, _rho.Start);
            var iz = DetectorBinning.WhichBin(dp.Position.Z, _z.Count - 1, _z.Delta, _z.Start);
            //double dw = previousDP.Weight * ops[_tissue.GetRegionIndex(dp.Position)].Mua / 
            //    (ops[_tissue.GetRegionIndex(dp.Position)].Mua + ops[_tissue.GetRegionIndex(dp.Position)].Mus);
            _pst = dp.StateFlag;
            _dw = previousDP.Weight;
            _nextDw = dp.Weight;
            AbsorbAction(ops[_tissue.GetRegionIndex(dp.Position)].Mua, ops[_tissue.GetRegionIndex(dp.Position)].Mus);
            var dum = _tissue.GetRegionIndex(dp.Position);
            Mean[ir, iz] += _dw / ops[_tissue.GetRegionIndex(dp.Position)].Mua;
            SecondMoment[ir, iz] += (_dw / ops[_tissue.GetRegionIndex(dp.Position)].Mua) *
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