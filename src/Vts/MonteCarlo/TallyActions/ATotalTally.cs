using System;
using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.TallyActions
{
    /// <summary>
    /// Implements IHistoryTally<double[,]>.  Tally for Absorption(rho,z).
    /// </summary>
    public class ATotalTally : IHistoryTally<double>
    {
        private ITissue _tissue;

        public ATotalTally(ITissue tissue, AbsorptionWeightingType awt)
        {
            _tissue = tissue;
            SetAbsorbAction(awt);
        }
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
            //double dw = previousDP.Weight * ops[_tissue.GetRegionIndex(dp.Position)].Mua / 
            //    (ops[_tissue.GetRegionIndex(dp.Position)].Mua + ops[_tissue.GetRegionIndex(dp.Position)].Mus);
            _pst = dp.StateFlag;
            _dw = previousDP.Weight;
            _nextDw = dp.Weight;
            AbsorbAction(ops[_tissue.GetRegionIndex(dp.Position)].Mua, ops[_tissue.GetRegionIndex(dp.Position)].Mus);
            Mean += _dw; 
            SecondMoment += _dw * _dw;
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
            Mean /=  numPhotons;

        }
        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true;
        }
        public double Mean { get; set; }
        public double SecondMoment { get; set; }

    }
}