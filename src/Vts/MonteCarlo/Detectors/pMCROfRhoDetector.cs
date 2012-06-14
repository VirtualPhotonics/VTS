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
    /// Implements IDetector&lt;double[]&gt;.  Tally for pMC estimation of reflectance 
    /// as a function of Rho.
    /// </summary>
    [KnownType(typeof(pMCROfRhoDetector))]
    public class pMCROfRhoDetector : IDetector<double[]> 
    {
        private IList<OpticalProperties> _referenceOps;
        private IList<OpticalProperties> _perturbedOps;
        private IList<int> _perturbedRegionsIndices;
        private double _rhoDelta;  // need to keep this because DoubleRange adjusts deltas automatically
        private bool _tallySecondMoment;
        private Func<IList<long>, IList<double>, IList<OpticalProperties>, double> _absorbAction;
        private AbsorptionWeightingType _awt;

        /// <summary>
        /// constructor for perturbation Monte Carlo reflectance as a function of rho detector input
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="tissue">tissue definition</param>
        /// <param name="perturbedOps">list of perturbed optical properties, indexing matches tissue indexing</param>
        /// <param name="perturbedRegionIndices">list of perturbed tissue region indices, indexing matches tissue indexing</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment info for error results</param>
        /// <param name="name">detector name</param>
        public pMCROfRhoDetector(
            DoubleRange rho,
            ITissue tissue,
            IList<OpticalProperties> perturbedOps,
            IList<int> perturbedRegionIndices,
            bool tallySecondMoment,
            String name)
        {
            Rho = rho;
            _tallySecondMoment = tallySecondMoment;
            Mean = new double[Rho.Count - 1]; // change from count -1
            SecondMoment = null;
            if (_tallySecondMoment)
            {
                SecondMoment = new double[Rho.Count - 1];
            }
            TallyType = "pMCROfRho";
            Name = name;
            _perturbedOps = perturbedOps;
            _referenceOps = tissue.Regions.Select(r => r.RegionOP).ToList();
            _perturbedRegionsIndices = perturbedRegionIndices;
            SetAbsorbAction(tissue.AbsorptionWeightingType);
            TallyCount = 0;
            _awt = tissue.AbsorptionWeightingType;
        }

        /// <summary>
        /// Returns a default instance of pMCMuaMusROfRhoDetector (for serialization purposes only)
        /// </summary>
        public pMCROfRhoDetector()
            : this(
            new DoubleRange(), 
            new MultiLayerTissue(), 
            new List<OpticalProperties>(), 
            new List<int>(), 
            true, // tallySecondMoment
            "pMCROfRho" )
        {
        }

        /// <summary>
        /// detector mean
        /// </summary>
        [IgnoreDataMember]
        public double[] Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember]
        public double[] SecondMoment { get; set; }
        /// <summary>
        /// detector identifier
        /// </summary>
        public string TallyType { get; set; }
        /// <summary>
        /// detector name, default uses TallyType, but can be user specified
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// number of time detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }
        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }

        /// <summary>
        /// Set the absorption to discrete or continuous
        /// </summary>
        /// <param name="awt">absorption weighting type</param>
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
        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            // no longer need trial code since DP weight for CAW now uses pathlength info after each collision to determine weight
            // trial code: overwrites dp.Weight 
            //if (_awt == AbsorptionWeightingType.Continuous)
            //{
            //    var trialWeight = 1.0;
            //    for (int i = 0; i < _referenceOps.Count; i++)
            //    {
            //        trialWeight *= Math.Exp(-_referenceOps[i].Mua * photon.History.SubRegionInfoList[i].PathLength);
            //    }
            //    photon.DP.Weight = trialWeight;
            //}
            // end trial code
            var ir = DetectorBinning.WhichBinExclusive(DetectorBinning.GetRho(photon.DP.Position.X, photon.DP.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
            if (ir != -1)
            {
                double weightFactor = _absorbAction(
                    photon.History.SubRegionInfoList.Select(c => c.NumberOfCollisions).ToList(),
                    photon.History.SubRegionInfoList.Select(p => p.PathLength).ToList(),
                    _perturbedOps);

                Mean[ir] += photon.DP.Weight * weightFactor;
                if (_tallySecondMoment)
                {
                    SecondMoment[ir] += photon.DP.Weight * weightFactor * photon.DP.Weight * weightFactor;
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
                    Math.Exp(-(perturbedOps[i].Mua - _referenceOps[i].Mua) * pathLength[i]); // mua pert
                if (numberOfCollisions[i] > 0) // mus pert
                {
                    // the following is more numerically stable
                    weightFactor *= Math.Pow(
                        (_perturbedOps[i].Mus / _referenceOps[i].Mus) * Math.Exp(-(_perturbedOps[i].Mus - _referenceOps[i].Mus) *
                            pathLength[i] / numberOfCollisions[i]),
                        numberOfCollisions[i]);
                }
                else
                {
                    weightFactor *= Math.Exp(-(_perturbedOps[i].Mus - _referenceOps[i].Mus) * pathLength[i]);
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
                            (_perturbedOps[i].Mus / _referenceOps[i].Mus) *
                                Math.Exp(-(_perturbedOps[i].Mus + _perturbedOps[i].Mua - _referenceOps[i].Mus - _referenceOps[i].Mua) *
                                pathLength[i] / numberOfCollisions[i]),
                            numberOfCollisions[i]);
                }
                else
                {
                    weightFactor *=
                        Math.Exp(-(_perturbedOps[i].Mus + _perturbedOps[i].Mua - _referenceOps[i].Mus - _referenceOps[i].Mua) *
                                pathLength[i]);
                }
            }
            return weightFactor;
        }

        /// <summary>
        /// method to normalize detector results after numPhotons launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * normalizationFactor;
                Mean[ir] /= areaNorm * numPhotons;
                // the above is pi(rmax*rmax-rmin*rmin) * rhoDelta * N
                if (_tallySecondMoment)
                {
                    SecondMoment[ir] /= areaNorm * areaNorm * numPhotons;
                }
            }
        }

        /// <summary>
        /// Method to determine if photon is within detector
        /// </summary>
        /// <param name="dp">photon data point</param>
        /// <returns>method always returns true</returns>
        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true; // or, possibly test for NA or confined position, etc
            // return (dp.StateFlag.Has(PhotonStateType.PseudoTransmissionDomainTopBoundary));
        }
    }
}
