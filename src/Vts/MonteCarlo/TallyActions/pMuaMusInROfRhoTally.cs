using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.TallyActions
{
    /// <summary>
    /// Implements ITerminationTally<double[]>.  Tally for pMC estimation of reflectance 
    /// as a function of Rho.
    /// </summary>
    // do I need classes pMuaInROfRhoTally and pMusInROfRhoTally?
    public class pMuaMusInROfRhoTally : HistoryTallyBase, ITerminationTally<double[]>
    {
        private DoubleRange _rho;
        private AbsorptionWeightingType _awt;
        private IList<OpticalProperties> _referenceOps;
        private IList<OpticalProperties> _perturbedOps;
        private IList<int> _perturbedRegionsIndices;
        private double _rhoDelta;  // need to keep this because DoubleRange adjusts deltas automatically
        // note: bins accommodate noncontiguous and also single bins
        private double[] _rhoCenters;
        private Func<IList<long>, double, IList<OpticalProperties>, double> _absorbAction;
        /// <summary>
        /// Tallies perturbed R(rho).  Instantiate with reference optical properties.  When
        /// method Tally invoked, perturbed optical properties passed.
        /// </summary>
        /// <param name="rho"></param>
        /// <param name="time"></param>
        /// <param name="awt"></param>
        /// <param name="referenceOps"></param>
        /// <param name="perturbedRegionIndices"></param>
        public pMuaMusInROfRhoTally(
            DoubleRange rho,
            ITissue tissue,
            IList<OpticalProperties> perturbedOps,
            IList<int> perturbedRegionIndices) : base(tissue)
        {
            _rho = rho;
            Mean = new double[_rho.Count - 1];
            SecondMoment = new double[_rho.Count - 1];
            _awt = tissue.AbsorptionWeightingType;
            _referenceOps = tissue.Regions.Select(r => r.RegionOP).ToList();
            _perturbedRegionsIndices = perturbedRegionIndices;
            SetAbsorbAction(_awt);
            if (_rho.Count - 1 == 1)
            {
                _rho.Start = _rho.Start - 0.1;
                _rhoDelta = 0.2;
                _rho.Stop = _rho.Start + _rhoDelta;
                _rhoCenters = new double[1] { _rho.Start };
            }
            else // put rhoCenters at rhos specified by user
            {
                _rhoDelta = _rho.Delta;
                _rhoCenters = new double[_rho.Count - 1];
                for (int i = 0; i < _rho.Count - 1; i++)
                {
                    _rhoCenters[i] = _rho.Start + i * _rhoDelta;
                }
            }
        }

        public double[] Mean { get; set; }
        public double[] SecondMoment { get; set; }

        protected override void SetAbsorbAction(AbsorptionWeightingType awt)
        {
            switch (awt)
            {
                // note: pMC is not applied to analog processing,
                // only DAW and CAW
                case AbsorptionWeightingType.Continuous:
                    _absorbAction = AbsorbContinuous;
                    break;
                case AbsorptionWeightingType.Discrete:
                default:
                    _absorbAction = AbsorbDiscrete;
                    break;
            }
        }

        public void Tally(PhotonDataPoint dp)
        {
            double weightFactor = 1.0;

            var totalTime = dp.SubRegionInfoList.Select((sub, i) =>
                DetectorBinning.GetTimeDelay(
                sub.PathLength,
                _referenceOps[i].N)  // time is based on reference optical properties
                ).Sum();
            
            double totalPathLengthInPerturbedRegions = 0.0;
            foreach (var i in _perturbedRegionsIndices)
            {
                totalPathLengthInPerturbedRegions += dp.SubRegionInfoList[i].PathLength;
            }

            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y),
                _rho.Delta, _rhoCenters);
            if (ir != -1)
            {
                weightFactor = _absorbAction(
                    dp.SubRegionInfoList.Select(c => c.NumberOfCollisions).ToList(),
                    totalPathLengthInPerturbedRegions,
                    _perturbedOps);
                Mean[ir] += dp.Weight * weightFactor;
                SecondMoment[ir] += dp.Weight * weightFactor * dp.Weight * weightFactor;
            }
        }
        private double AbsorbContinuous(IList<long> numberOfCollisions, double totalPathLengthInPerturbedRegions, IList<OpticalProperties> perturbedOps)
        {
            double weightFactor = 1.0;

            foreach (var i in _perturbedRegionsIndices)
            {
                weightFactor *=
                    Math.Pow(
                        (_perturbedOps[i].Mus / _referenceOps[i].Mus),
                        numberOfCollisions[i]) *
                    Math.Exp(-(_perturbedOps[i].Mus - _referenceOps[i].Mus) *
                        totalPathLengthInPerturbedRegions);
            }
            return weightFactor;
        }
        private double AbsorbDiscrete(IList<long> numberOfCollisions, double totalPathLengthInPerturbedRegions, IList<OpticalProperties> perturbedOps)
        {
            double weightFactor = 1.0;

            foreach (var i in _perturbedRegionsIndices)
            {
                weightFactor *=
                    Math.Pow(
                        (_perturbedOps[i].Mus / _referenceOps[i].Mus),
                        numberOfCollisions[i]) *
                    Math.Exp(-(_perturbedOps[i].Mus + _perturbedOps[i].Mua - _referenceOps[i].Mus - _referenceOps[i].Mua) *
                        totalPathLengthInPerturbedRegions);

            }
            return weightFactor;
        }
        public void Normalize(long numPhotons)
        {
            for (int ir = 0; ir < _rho.Count - 1; ir++)
            {
                Mean[ir] /=
                    2 * Math.PI * _rhoCenters[ir] * _rhoDelta * numPhotons;
                // the above is pi(rmax*rmax-rmin*rmin) * timeDelta * N

            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutTop);
        }
    }
}
