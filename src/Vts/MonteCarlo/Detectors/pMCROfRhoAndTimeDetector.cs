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
    /// <summary>
    /// Implements ITerminationTally&lt;double[,]&gt;.  Tally for pMC estimation of reflectance 
    /// as a function of Rho and Time.  Perturbations of just mua or mus alone are also
    /// handled by this class.
    /// </summary>
    [KnownType(typeof(pMCROfRhoAndTimeDetector))]
    public class pMCROfRhoAndTimeDetector : IpMCSurfaceDetector<double[,]>
    {
        private AbsorptionWeightingType _awt;
        private IList<OpticalProperties> _referenceOps;
        private IList<OpticalProperties> _perturbedOps;
        private IList<int> _perturbedRegionsIndices;
        private bool _tallySecondMoment;
        private Func<IList<long>, IList<double>, IList<OpticalProperties>, double> _absorbAction;

        /// <summary>
        /// Returns an instance of pMCROfRhoAndTimeDetector. Tallies perturbed R(rho,time). Instantiate with reference optical properties. When
        /// method Tally invoked, perturbed optical properties passed.
        /// </summary>
        /// <param name="rho"></param>
        /// <param name="time"></param>
        /// <param name="tissue"></param>
        /// <param name="perturbedOps"></param>
        /// <param name="perturbedRegionIndices"></param>
        public pMCROfRhoAndTimeDetector(
            DoubleRange rho,
            DoubleRange time,
            ITissue tissue,
            IList<OpticalProperties> perturbedOps,
            IList<int> perturbedRegionIndices,
            bool tallySecondMoment,
            String name)
        {
            Rho = rho;
            Time = time;
            _tallySecondMoment = tallySecondMoment;
            Mean = new double[Rho.Count - 1, Time.Count - 1];
            SecondMoment = null;
            if (_tallySecondMoment)
            {
                SecondMoment = new double[Rho.Count - 1, Time.Count - 1];
            }
            TallyType = TallyType.pMCROfRhoAndTime;
            Name = name;
            _awt = tissue.AbsorptionWeightingType;
            _referenceOps = tissue.Regions.Select(r => r.RegionOP).ToList();
            _perturbedOps = perturbedOps;
            _perturbedRegionsIndices = perturbedRegionIndices;
            SetAbsorbAction(_awt);
            TallyCount = 0;
        }

        /// <summary>
        /// Returns a default instance of pMCMuaMusROfRhoAndTimeDetector (for serialization purposes only)
        /// </summary>
        public pMCROfRhoAndTimeDetector()
            : this(
            new DoubleRange(), 
            new DoubleRange(), 
            new MultiLayerTissue(), 
            new List<OpticalProperties>(), 
            new List<int>(),
            true, // tallySecondMoment
            TallyType.pMCROfRhoAndTime.ToString())
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

        public void Tally(PhotonDataPoint dp, CollisionInfo infoList)
        {
            var totalTime = dp.TotalTime;
            var it = DetectorBinning.WhichBinExclusive(totalTime, Time.Count - 1, Time.Delta, Time.Start);
            var ir = DetectorBinning.WhichBinExclusive(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y),
                Rho.Count - 1, Rho.Delta, Rho.Start);
            if ((ir != -1) && (it != -1))
            {
                var weightFactor = _absorbAction(
                    infoList.Select(c => c.NumberOfCollisions).ToList(),
                    infoList.Select(p => p.PathLength).ToList(),
                    _perturbedOps);
                Mean[ir, it] += dp.Weight * weightFactor;
                if (_tallySecondMoment)
                {
                    SecondMoment[ir, it] += dp.Weight * weightFactor * dp.Weight * weightFactor;
                }
                TallyCount++;
            }
        }

        private double AbsorbContinuous(IList<long> numberOfCollisions, IList<double> pathLength, IList<OpticalProperties> perturbedOps)
        {
            double weightFactor = 1.0;

            foreach (var i in _perturbedRegionsIndices)
            {
                weightFactor *=
                    (Math.Exp(-perturbedOps[i].Mua * pathLength[i]) / Math.Exp(-_referenceOps[i].Mua * pathLength[i])); // mua pert
                if (numberOfCollisions[i] > 0)
                {
                    // following is more numerically stable
                    weightFactor *= Math.Pow(
                        (perturbedOps[i].Mus / _referenceOps[i].Mus) * Math.Exp(-(perturbedOps[i].Mus - _referenceOps[i].Mus) *
                            pathLength[i] / numberOfCollisions[i]),
                        numberOfCollisions[i]);
                }
                else
                {
                    weightFactor *= Math.Exp(-(perturbedOps[i].Mus - _referenceOps[i].Mus) * pathLength[i]);
                }
            }
            return weightFactor;
        }

        private double AbsorbDiscrete(IList<long> numberOfCollisions, IList<double> pathLength, IList<OpticalProperties> perturbedOps)
        {
            double weightFactor = 1.0;

            foreach (var i in _perturbedRegionsIndices)
            {
                if (numberOfCollisions[i] > 0)
                {
                    weightFactor *=
                        Math.Pow(
                            (perturbedOps[i].Mus / _referenceOps[i].Mus) *
                                Math.Exp(-(perturbedOps[i].Mus + perturbedOps[i].Mua -
                                   _referenceOps[i].Mus - _referenceOps[i].Mua) * pathLength[i] / numberOfCollisions[i]),
                            numberOfCollisions[i]);
                }
                else
                {
                    weightFactor *=
                        Math.Exp(-(perturbedOps[i].Mus + perturbedOps[i].Mua -
                                   _referenceOps[i].Mus - _referenceOps[i].Mua) * pathLength[i]);
                }
            }
            return weightFactor;
        }

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2 * Math.PI * Rho.Delta * Time.Delta;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (int it = 0; it < Time.Count - 1; it++)
                {
                    var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * normalizationFactor;
                    Mean[ir, it] /= areaNorm * numPhotons;
                    // the above is pi(rmax*rmax-rmin*rmin) * timeDelta * N
                    if (_tallySecondMoment)
                    {
                        SecondMoment[ir, it] /= areaNorm * areaNorm * numPhotons;
                    }
                }
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true; // or, possibly test for NA or confined position, etc
            //return (dp.StateFlag.Has(PhotonStateType.PseudoTransmissionDomainTopBoundary));
        }
    }
}
