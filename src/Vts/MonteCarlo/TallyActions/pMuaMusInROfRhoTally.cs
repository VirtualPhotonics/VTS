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
    public class pMuaMusInROfRhoTally : ITerminationTally<double[]>
    {
        private DoubleRange _rho;
        private AbsorptionWeightingType _awt;
        private IList<OpticalProperties> _referenceOps;
        private IList<int> _perturbedRegionsIndices;
        private double _rhoDelta;  // need to keep this because DoubleRange adjusts deltas automatically
        // note: bins accommodate noncontiguous and also single bins
        private double[] _rhoCenters;
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
            AbsorptionWeightingType awt,
            IList<OpticalProperties> referenceOps,
            IList<int> perturbedRegionIndices)
        {
            _rho = rho;
            Mean = new double[_rho.Count - 1];
            SecondMoment = new double[_rho.Count - 1];
            _awt = awt;
            _referenceOps = referenceOps;
            _perturbedRegionsIndices = perturbedRegionIndices;
            SetAbsorbAction(awt);
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

        // should following code be put in common class?
        public Action AbsorbAction { get; private set; }
        private void SetAbsorbAction(AbsorptionWeightingType awt)
        {
            switch (awt)
            {
                // note: pMC is not applied to analog processing,
                // only DAW and CAW
                case AbsorptionWeightingType.Continuous:
                    AbsorbAction = AbsorbContinuous;
                    break;
                case AbsorptionWeightingType.Discrete:
                default:
                    AbsorbAction = AbsorbDiscrete;
                    break;
            }
        }
        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutTop);
        }
        double _weightFactor = 1.0;
        PhotonDataPoint _dp;
        IList<OpticalProperties> _perturbedOps;
        double _totalPathLengthInPerturbedRegions = 0.0;
        public void Tally(PhotonDataPoint dp, IList<OpticalProperties> perturbedOps)
        {
            //double weightFactor = 1.0;
            _dp = dp;
            _perturbedOps = perturbedOps;
            var totalTime = dp.SubRegionInfoList.Select((sub, i) =>
                DetectorBinning.GetTimeDelay(
                sub.PathLength,
                _referenceOps[i].N)  // time is based on reference optical properties
                ).Sum();
            //double totalPathLengthInPerturbedRegions = 0.0;
            foreach (var i in _perturbedRegionsIndices)
            {
                _totalPathLengthInPerturbedRegions += dp.SubRegionInfoList[i].PathLength;
            }
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y),
                _rho.Delta, _rhoCenters);
            if (ir != -1)
            {
                AbsorbAction();
                Mean[ir] += dp.Weight * _weightFactor;
                SecondMoment[ir] += dp.Weight * _weightFactor * dp.Weight * _weightFactor;
            }
        }
        public void AbsorbContinuous()
        {
            foreach (var i in _perturbedRegionsIndices)
            {
                _weightFactor *=
                    Math.Pow(
                        (_perturbedOps[i].Mus / _referenceOps[i].Mus),
                        _dp.SubRegionInfoList[i].NumberOfCollisions) *
                    Math.Exp(-(_perturbedOps[i].Mus - _referenceOps[i].Mus) *
                        _totalPathLengthInPerturbedRegions);
            }
        }
        public void AbsorbDiscrete()
        {
            foreach (var i in _perturbedRegionsIndices)
            {
                _weightFactor *=
                    Math.Pow(
                        (_perturbedOps[i].Mus / _referenceOps[i].Mus),
                        _dp.SubRegionInfoList[i].NumberOfCollisions) *
                    Math.Exp(-(_perturbedOps[i].Mus + _perturbedOps[i].Mua - _referenceOps[i].Mus - _referenceOps[i].Mua) *
                        _totalPathLengthInPerturbedRegions);

            }
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
    }
}
