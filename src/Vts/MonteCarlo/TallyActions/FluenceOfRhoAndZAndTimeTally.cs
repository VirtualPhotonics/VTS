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
    /// Note: this tally currently only works with discrete absorption weighting and analog
    /// </summary>
    public class FluenceOfRhoAndZAndTimeTally : HistoryTallyBase, IHistoryTally<double[, ,]>
    {
        private DoubleRange _rho;
        private DoubleRange _z;
        private DoubleRange _time;
        private Func<double, double, double, double, PhotonStateType, double> _absorbAction;

        public FluenceOfRhoAndZAndTimeTally(DoubleRange rho, DoubleRange z, DoubleRange time, ITissue tissue)
            : base(tissue)
        {
            _rho = rho;
            _z = z;
            _time = time;

            Mean = new double[_rho.Count - 1, _z.Count - 1, _time.Count - 1];
            SecondMoment = new double[_rho.Count - 1, _z.Count - 1, _time.Count - 1];
            TallyType = TallyType.FluenceOfRhoAndZAndTime;
        }

        public double[, ,] Mean { get; set; }
        public double[, ,] SecondMoment { get; set; }
        public TallyType TallyType { get; set; }

        protected override void SetAbsorbAction(AbsorptionWeightingType awt)
        {
            switch (awt)
            {
                case AbsorptionWeightingType.Analog:
                    _absorbAction = AbsorbAnalog;
                    break;
                //case AbsorptionWeightingType.Continuous:
                //    Absorb = AbsorbContinuous;
                //    break;
                case AbsorptionWeightingType.Discrete:
                    _absorbAction = AbsorbDiscrete;
                    break;
                default:
                    throw new ArgumentException("AbsorptionWeightingType not set");
            }
        }

        public void Tally(PhotonDataPoint previousDP, PhotonDataPoint dp)
        {
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), _rho.Count - 1, _rho.Delta, _rho.Start);
            var iz = DetectorBinning.WhichBin(dp.Position.Z, _z.Count - 1, _z.Delta, _z.Start);
            var it = DetectorBinning.WhichBin(dp.TotalTime, _time.Count - 1, _time.Delta, _time.Start);

            var weight = _absorbAction(
                _ops[_tissue.GetRegionIndex(dp.Position)].Mua,
                _ops[_tissue.GetRegionIndex(dp.Position)].Mus,
                previousDP.Weight,
                dp.Weight,
                dp.StateFlag);

            var regionIndex = _tissue.GetRegionIndex(dp.Position);

            Mean[ir, iz, it] += weight / _ops[regionIndex].Mua;
            SecondMoment[ir, iz, it] += (weight / _ops[regionIndex].Mua) * (weight / _ops[regionIndex].Mua);
        }

        private double AbsorbAnalog(double mua, double mus, double previousWeight, double weight, PhotonStateType photonStateType)
        {
            if (photonStateType != PhotonStateType.Absorbed)
            {
                weight = 0.0;
            }
            else
            {
                weight = previousWeight * mua / (mua + mus);
            }
            return weight;
        }

        private double AbsorbDiscrete(double mua, double mus, double previousWeight, double weight, PhotonStateType photonStateType)
        {
            if (previousWeight == weight) // pseudo collision, so no tally
            {
                weight = 0.0;
            }
            else
            {
                weight = previousWeight * mua / (mua + mus);
            }
            return weight;
        }

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * _rho.Delta * _rho.Delta * _z.Delta * _time.Delta * numPhotons;
            for (int ir = 0; ir < _rho.Count - 1; ir++)
            {
                for (int iz = 0; iz < _z.Count - 1; iz++)
                {
                    for (int it = 0; it < _time.Count - 1; it++)
                    {
                        Mean[ir, iz, it] /= (ir + 0.5) * normalizationFactor;
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