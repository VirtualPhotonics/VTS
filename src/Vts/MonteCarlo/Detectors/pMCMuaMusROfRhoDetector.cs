using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements ITerminationTally<double[]>.  Tally for pMC estimation of reflectance 
    /// as a function of Rho.
    /// </summary>
    // do I need classes pMuaInROfRhoTally and pMusInROfRhoTally?
    public class pMCMuaMusROfRhoDetector : IpMCTerminationDetector<double[]>
    {
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
        public pMCMuaMusROfRhoDetector(
            DoubleRange rho,
            ITissue tissue,
            IList<OpticalProperties> perturbedOps,
            IList<int> perturbedRegionIndices)
        {
            Rho = rho;
            Mean = new double[Rho.Count - 1];
            SecondMoment = new double[Rho.Count - 1];
            TallyType = TallyType.pMuaMusInROfRho;
            _perturbedOps = perturbedOps;
            _referenceOps = tissue.Regions.Select(r => r.RegionOP).ToList();
            _perturbedRegionsIndices = perturbedRegionIndices;
            SetAbsorbAction(tissue.AbsorptionWeightingType);
            if (Rho.Count - 1 == 1)
            {
                Rho.Start = Rho.Start - 0.1;
                _rhoDelta = 0.2;
                Rho.Stop = Rho.Start + _rhoDelta;
                _rhoCenters = new double[1] { Rho.Start };
            }
            else // put rhoCenters at rhos specified by user
            {
                _rhoDelta = Rho.Delta;
                _rhoCenters = new double[Rho.Count - 1];
                for (int i = 0; i < Rho.Count - 1; i++)
                {
                    _rhoCenters[i] = Rho.Start + i * _rhoDelta;
                }
            }
            TallyCount = 0;
        }

        [IgnoreDataMember]
        public double[] Mean { get; set; }

        [IgnoreDataMember]
        public double[] SecondMoment { get; set; }

        public TallyType TallyType { get; set; }

        public long TallyCount { get; set; }

        public DoubleRange Rho { get; set; }
        
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
            double totalPathLengthInPerturbedRegions = 0.0;
            foreach (var i in _perturbedRegionsIndices)
            {
                totalPathLengthInPerturbedRegions += infoList[i].PathLength;
            }

            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), Rho.Delta, _rhoCenters);
            if (ir != -1)
            {
                double weightFactor = _absorbAction(
                    infoList.Select(c => c.NumberOfCollisions).ToList(),
                    totalPathLengthInPerturbedRegions,
                    _perturbedOps);

                Mean[ir] += dp.Weight * weightFactor;
                SecondMoment[ir] += dp.Weight * weightFactor * dp.Weight * weightFactor;
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
            for (int ir = 0; ir < Rho.Count - 1; ir++)
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
