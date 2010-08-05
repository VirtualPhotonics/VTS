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
        private double _rhoDelta;  // need to keep these two because DoubleRange adjusts deltas automatically
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
            Mean = new double[_rho.Count, _time.Count];
            SecondMoment = new double[_rho.Count, _time.Count];
            _awt = awt;
            _referenceOps = referenceOps;
            _perturbedRegionsIndices = perturbedRegionIndices;
            if (_rho.Count == 1)
            {
                _rho.Start = _rho.Start - 0.1;
                _rhoDelta = 0.2;
                _rho.Stop = _rho.Start + _rhoDelta;
                _rhoCenters = new double[1] { _rho.Start + _rhoDelta / 2 };
                _timeDelta = _time.Delta;
                _timeCenters = new double[_time.Count];
                for (int i = 0; i < _time.Count; i++)
                {
                    _timeCenters[i] = _time.Start + (i + 1) * _timeDelta / 2;
                }
            }
            if (_time.Count == 1)
            {
                _time.Start = _time.Start - 0.0025;
                _timeDelta = 0.005;
                _time.Stop = _time.Start + _timeDelta;
                _timeCenters = new double[1] { _time.Start + _timeDelta / 2 };
                _rhoDelta = _rho.Delta;
                _rhoCenters = new double[_rho.Count];
                for (int i = 0; i < _rho.Count; i++)
                {
                    _rhoCenters[i] = _rho.Start + (i + 1) * _rhoDelta / 2;  
                }
            }
        }

        public double[,] Mean { get; set; }
        public double[,] SecondMoment { get; set; }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutTop);
        }

        public void Tally(PhotonDataPoint dp, IList<OpticalProperties> perturbedOps)
        {
            double weightFactor = 0.0;
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
            var it = DetectorBinning.WhichBin(totalTime, _time.Count, _time.Delta, _time.Start);
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y),
                _rho.Count, _rho.Delta, _rho.Start);
            if (_rho.Count == 1)
            {
                ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y),
                    _rhoDelta, _rhoCenters);
            }
            if (_time.Count == 1)
            {
                it = DetectorBinning.WhichBin(totalTime, _timeDelta, _timeCenters);
            }
            if ((ir != -1) && (it != -1))
            {
                if (_awt == AbsorptionWeightingType.Discrete) // DC: how do I set this so not going thru for each tally
                {
                    foreach (var i in _perturbedRegionsIndices)
                    {
                        weightFactor +=
                            Math.Pow(
                                (perturbedOps[i].Mus / _referenceOps[i].Mus),
                                dp.SubRegionInfoList[i].NumberOfCollisions) *
                            Math.Exp(-(perturbedOps[i].Mus + perturbedOps[i].Mua - _referenceOps[i].Mus - _referenceOps[i].Mua) *
                                totalPathLengthInPerturbedRegions);
                    }
                }
                else // CAW
                {
                    foreach (var i in _perturbedRegionsIndices)
                    {
                        weightFactor +=
                            Math.Pow(
                                (perturbedOps[i].Mus / _referenceOps[i].Mus),
                                dp.SubRegionInfoList[i].NumberOfCollisions) *
                            Math.Exp(-(perturbedOps[i].Mus - _referenceOps[i].Mus) *
                                totalPathLengthInPerturbedRegions);
                    }
                }
                Mean[ir, it] += dp.Weight * weightFactor;
                SecondMoment[ir, it] += dp.Weight * weightFactor * dp.Weight * weightFactor;
            }
        }
        public void Normalize(long numPhotons)
        {
            for (int ir = 0; ir < _rho.Count; ir++)
            {
                for (int it = 0; it < _time.Count; it++)
                {
                    Mean[ir, it] /=
                        2 * Math.PI * _rhoCenters[ir] * _rhoDelta  * _timeDelta * numPhotons;
                    // the above is pi(rmax*rmax-rmin*rmin) * timeDelta * N
                }
            }
        }
    }
}
