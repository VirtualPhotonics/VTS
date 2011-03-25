using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Detectors
{
    [KnownType(typeof(pMCMuaMusROfRhoAndTimeDetector))]
    /// <summary>
    /// Implements ITerminationTally<double[,]>.  Tally for pMC estimation of reflectance 
    /// as a function of Rho and Time.  Perturbations of just mua or mus alone are also
    /// handled by this class.
    /// </summary>
    public class pMCMuaMusROfRhoAndTimeDetector : IpMCTerminationDetector<double[,]>
    {
        private AbsorptionWeightingType _awt;
        private IList<OpticalProperties> _referenceOps;
        private IList<OpticalProperties> _perturbedOps;
        private IList<int> _perturbedRegionsIndices;
        // need next two because DoubleRange adjusts deltas automatically
        private double _rhoDelta;
        private double _timeDelta;
        // note: bins accommodate noncontiguous and also single bins
        private double[] _rhoCenters;
        private double[] _timeCenters;
        private Func<IList<long>, double, IList<OpticalProperties>, double> _absorbAction;

        /// <summary>
        /// Returns an instance of pMCMuaMusROfRhoAndTimeDetector. Tallies perturbed R(rho,time). Instantiate with reference optical properties. When
        /// method Tally invoked, perturbed optical properties passed.
        /// </summary>
        /// <param name="rho"></param>
        /// <param name="time"></param>
        /// <param name="tissue"></param>
        /// <param name="perturbedOps"></param>
        /// <param name="perturbedRegionIndices"></param>
        public pMCMuaMusROfRhoAndTimeDetector(
            DoubleRange rho,
            DoubleRange time,
            ITissue tissue,
            IList<OpticalProperties> perturbedOps,
            IList<int> perturbedRegionIndices,
            String name)
        {
            Rho = rho;
            Time = time;
            // save delta because DoubleRange readjusts when Start,Stop,Count changes
            _rhoDelta = Rho.Delta;
            _timeDelta = Time.Delta;
            Mean = new double[Rho.Count - 1, Time.Count - 1];
            SecondMoment = new double[Rho.Count - 1, Time.Count - 1];
            TallyType = TallyType.pMCMuaMusROfRhoAndTime;
            Name = name;
            _awt = tissue.AbsorptionWeightingType;
            _referenceOps = tissue.Regions.Select(r => r.RegionOP).ToList();
            _perturbedOps = perturbedOps;
            _perturbedRegionsIndices = perturbedRegionIndices;
            SetAbsorbAction(_awt);
            TallyCount = 0;
            // problem: the gui defines the rhos and times with the centers,
            // but in the usual tally definition, the rhos and times define
            // the extent of the bin
            // so currently assuming if either only 1 rho bin or 1 time bin,
            // then gui call, otherwise regular tally.  Need to fix!
            if (Rho.Count - 1 == 1)
            {
                Rho.Start = Rho.Start - 0.1;
                _rhoDelta = 0.2;
                Rho.Stop = Rho.Start + _rhoDelta;
                _rhoCenters = new double[1] { Rho.Start };
                _timeCenters = new double[Time.Count - 1];
                for (int i = 0; i < Time.Count - 1; i++)
                {
                    _timeCenters[i] = Time.Start + i * _timeDelta;
                }
            }
            else
            {
                _rhoCenters = new double[Rho.Count - 1];
                for (int i = 0; i < Rho.Count - 1; i++)
                {
                    _rhoCenters[i] = Rho.Start + i * _rhoDelta + _rhoDelta / 2;
                }
            }
            if (Time.Count - 1 == 1)
            {
                Time.Start = Time.Start - 0.0025;
                _timeDelta = 0.005;
                Time.Stop = Time.Start + _timeDelta;
                _timeCenters = new double[1] { Time.Start };
                _rhoCenters = new double[Rho.Count - 1];
                for (int i = 0; i < Rho.Count - 1; i++)
                {
                    _rhoCenters[i] = Rho.Start + i * _rhoDelta;
                }
            }
            else
            {
                _timeCenters = new double[Time.Count - 1];
                for (int i = 0; i < Time.Count - 1; i++)
                {
                    _timeCenters[i] = Time.Start + i * _timeDelta + _timeDelta / 2;
                }
            }
        }

        /// <summary>
        /// Returns a default instance of pMCMuaMusROfRhoAndTimeDetector (for serialization purposes only)
        /// </summary>
        public pMCMuaMusROfRhoAndTimeDetector()
            : this(
            new DoubleRange(), 
            new DoubleRange(), 
            new MultiLayerTissue(), 
            new List<OpticalProperties>(), 
            new List<int>(),
            TallyType.pMCMuaMusROfRhoAndTime.ToString())
        {
        }

        [IgnoreDataMember]
        public double[,] Mean { get; set; }

        [IgnoreDataMember]
        public double[,] SecondMoment { get; set; }

        public TallyType TallyType { get; set; }

        public String Name { get; set; }

        public long TallyCount { get; set; }

        public DoubleRange Rho { get; set; }

        public DoubleRange Time { get; set; }

        protected void SetAbsorbAction(AbsorptionWeightingType awt)
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

        public void Tally(PhotonDataPoint dp, IList<SubRegionCollisionInfo> infoList)
        {
            var totalTime = dp.TotalTime;
            double totalPathLengthInPerturbedRegions = 0.0;
            foreach (var i in _perturbedRegionsIndices)
            {
                totalPathLengthInPerturbedRegions += infoList[i].PathLength;
            }
            var it = DetectorBinning.WhichBin(totalTime, Time.Count - 1, Time.Delta, Time.Start);
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y),
                Rho.Count - 1, Rho.Delta, Rho.Start);
            if (Rho.Count - 1 == 1)
            {
                ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y),
                    _rhoDelta, _rhoCenters);
            }
            if (Time.Count - 1 == 1)
            {
                it = DetectorBinning.WhichBin(totalTime, _timeDelta, _timeCenters);
            }
            if ((ir != -1) && (it != -1))
            {
                var weightFactor = _absorbAction(
                    infoList.Select(c => c.NumberOfCollisions).ToList(),
                    totalPathLengthInPerturbedRegions,
                    _perturbedOps);
                Mean[ir, it] += dp.Weight * weightFactor;
                SecondMoment[ir, it] += dp.Weight * weightFactor * dp.Weight * weightFactor;
                TallyCount++;
            }
        }

        private double AbsorbContinuous(IList<long> numberOfCollisions, double totalPathLengthInPerturbedRegions, IList<OpticalProperties> perturbedOps)
        {
            double weightFactor = 1.0;

            foreach (var i in _perturbedRegionsIndices)
            {
                weightFactor *=
                    Math.Pow(
                        (perturbedOps[i].Mus / _referenceOps[i].Mus),
                        numberOfCollisions[i]) *
                    Math.Exp(-(perturbedOps[i].Mus - _referenceOps[i].Mus) *
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
                        (perturbedOps[i].Mus / _referenceOps[i].Mus),
                        numberOfCollisions[i]) *
                    Math.Exp(-((perturbedOps[i].Mus + perturbedOps[i].Mua) -
                               (_referenceOps[i].Mus + _referenceOps[i].Mua)) *
                        totalPathLengthInPerturbedRegions);
            }
            return weightFactor;
        }

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2 * Math.PI * _rhoDelta * _timeDelta * numPhotons;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (int it = 0; it < Time.Count - 1; it++)
                {
                    Mean[ir, it] /= _rhoCenters[ir] * normalizationFactor;
                    // the above is pi(rmax*rmax-rmin*rmin) * timeDelta * N
                }
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutTop);
        }
    }
}
