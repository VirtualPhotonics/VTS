using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.TallyActions
{    
    /// <summary>
    /// Implements ITerminationTally<double[,]>.  Tally for pMC estimation of reflectance 
    /// as a function of Rho and Time.
    /// </summary>
    // do I need classes pMuaInROfRhoAndTimeTally and pMusInROfRhoAndTimeTally?
    public class pMuaMusInROfRhoAndTimeTally : ITerminationTally<double[,]>
    {
        private DoubleRange _rho;
        private DoubleRange _time;
        private AbsorptionWeightingType _awt;
        private IList<OpticalProperties> _referenceOps;
        private IList<int> _perturbedRegionsIndices;
        // need next two because DoubleRange adjusts deltas automatically
        private double _rhoDelta;  
        private double _timeDelta;
        // note: bins accommodate noncontiguous and also single bins
        private double[] _rhoCenters;
        private double[] _timeCenters;
        /// <summary>
        /// Tallies perturbed R(rho,time).  Instantiate with reference optical properties.  When
        /// method Tally invoked, perturbed optical properties passed.
        /// </summary>
        /// <param name="rho"></param>
        /// <param name="time"></param>
        /// <param name="awt"></param>
        /// <param name="referenceOps"></param>
        /// <param name="perturbedRegionIndices"></param>
        public pMuaMusInROfRhoAndTimeTally(
            DoubleRange rho,
            DoubleRange time,
            AbsorptionWeightingType awt,
            IList<OpticalProperties> referenceOps,
            IList<int> perturbedRegionIndices)
        {
            _rho = rho;
            _time = time;
            // save delta because DoubleRange readjusts when Start,Stop,Count changes
            _rhoDelta = _rho.Delta;
            _timeDelta = _time.Delta;
            Mean = new double[_rho.Count - 1, _time.Count - 1];
            SecondMoment = new double[_rho.Count - 1, _time.Count - 1];
            _awt = awt;
            _referenceOps = referenceOps;
            _perturbedRegionsIndices = perturbedRegionIndices;
            SetAbsorbAction(awt);
            // problem: the gui defines the rhos and times with the centers,
            // but in the usual tally definition, the rhos and times define
            // the extent of the bin
            // so currently assuming if either only 1 rho bin or 1 time bin,
            // then gui call, otherwise regular tally.  Need to fix!
            if (_rho.Count - 1 == 1)
            {
                _rho.Start = _rho.Start - 0.1;
                _rhoDelta = 0.2;
                _rho.Stop = _rho.Start + _rhoDelta;
                _rhoCenters = new double[1] { _rho.Start };
                _timeCenters = new double[_time.Count - 1];
                for (int i = 0; i < _time.Count - 1; i++)
                {
                    _timeCenters[i] = _time.Start + i * _timeDelta;
                }
            }
            else
            {
                _rhoCenters = new double[_rho.Count - 1];
                for (int i = 0; i < _rho.Count - 1; i++)
                {
                    _rhoCenters[i] = _rho.Start + i * _rhoDelta + _rhoDelta / 2;
                }
            }
            if (_time.Count - 1 == 1)
            {
                _time.Start = _time.Start - 0.0025;
                _timeDelta = 0.005;
                _time.Stop = _time.Start + _timeDelta;
                _timeCenters = new double[1] { _time.Start };
                _rhoCenters = new double[_rho.Count - 1];
                for (int i = 0; i < _rho.Count - 1; i++)
                {
                    _rhoCenters[i] = _rho.Start + i * _rhoDelta;
                }
            }
            else
            {
                _timeCenters = new double[_time.Count - 1];
                for (int i = 0; i < _time.Count - 1; i++)
                {
                    _timeCenters[i] = _time.Start + i * _timeDelta + _timeDelta / 2;
                }
            }
        }

        public double[,] Mean { get; set; }
        public double[,] SecondMoment { get; set; }

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
        double _weightFactor;
        PhotonDataPoint _dp;
        IList<OpticalProperties> _perturbedOps;
        double _totalPathLengthInPerturbedRegions = 0.0;
        public void Tally(PhotonDataPoint dp, IList<OpticalProperties> perturbedOps)
        {
            //double weightFactor = 1.0;
            _weightFactor = 1.0;
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
            var it = DetectorBinning.WhichBin(totalTime, _time.Count - 1, _time.Delta, _time.Start);
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y),
                _rho.Count - 1, _rho.Delta, _rho.Start);
            if (_rho.Count - 1 == 1)
            {
                ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y),
                    _rhoDelta, _rhoCenters);
            }
            if (_time.Count - 1 == 1)
            {
                it = DetectorBinning.WhichBin(totalTime, _timeDelta, _timeCenters);
            }
            if ((ir != -1) && (it != -1))
            {
                AbsorbAction();
                Mean[ir, it] += dp.Weight * _weightFactor;
                SecondMoment[ir, it] += dp.Weight * _weightFactor * dp.Weight * _weightFactor;
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
                    Math.Exp(-((_perturbedOps[i].Mus + _perturbedOps[i].Mua) -
                               (_referenceOps[i].Mus + _referenceOps[i].Mua)) *
                        _totalPathLengthInPerturbedRegions);
            }
        }
        public void Normalize(long numPhotons)
        {
            for (int ir = 0; ir < _rho.Count - 1; ir++)
            {
                for (int it = 0; it < _time.Count - 1; it++)
                {
                    Mean[ir, it] /=
                        2 * Math.PI * _rhoCenters[ir] * _rhoDelta  * _timeDelta * numPhotons;
                    // the above is pi(rmax*rmax-rmin*rmin) * timeDelta * N
                }
            }
        }
    }
}
