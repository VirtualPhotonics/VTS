using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Numerics;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements IDetector&lt;Complex[]&gt;.  Tally for pMC estimation of reflectance 
    /// as a function of Fx.
    /// </summary>
    [KnownType(typeof(pMCROfFxDetector))]
    public class pMCROfFxDetector : IDetector<Complex[]>
    {
        private IList<OpticalProperties> _referenceOps;
        private IList<OpticalProperties> _perturbedOps;
        private IList<int> _perturbedRegionsIndices;
        private bool _tallySecondMoment;
        private Func<IList<long>, IList<double>, IList<OpticalProperties>, double> _absorbAction;
        private AbsorptionWeightingType _awt;
        private double[] _fxArray;

        /// <summary>
        /// constructor for perturbation Monte Carlo reflectance as a function of spatial frequency input
        /// </summary>
        /// <param name="fx">fx binning</param>
        /// <param name="tissue">tissue definition</param>
        /// <param name="perturbedOps">list of perturbed optical properties, indexing matches tissue indexing</param>
        /// <param name="perturbedRegionIndices">list of perturbed tissue region indices, indexing matches tissue indexing</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment info for error results</param>
        /// <param name="name">detector name</param>
        public pMCROfFxDetector(
            DoubleRange fx,
            ITissue tissue,
            IList<OpticalProperties> perturbedOps,
            IList<int> perturbedRegionIndices,
            bool tallySecondMoment,
            String name)
        {
            Fx = fx;
            _fxArray = fx.AsEnumerable().ToArray();
            _tallySecondMoment = tallySecondMoment;
            Mean = new Complex[Fx.Count];
            SecondMoment = null;
            if (_tallySecondMoment)
            {
                SecondMoment = new Complex[Fx.Count];
            }
            TallyType = TallyType.pMCROfFx;
            Name = name;
            _perturbedOps = perturbedOps;
            _referenceOps = tissue.Regions.Select(r => r.RegionOP).ToList();
            _perturbedRegionsIndices = perturbedRegionIndices;
            SetAbsorbAction(tissue.AbsorptionWeightingType);
            TallyCount = 0;
            _awt = tissue.AbsorptionWeightingType;
        }

        /// <summary>
        /// Returns a default instance of pMCROfFxDetector (for serialization purposes only)
        /// </summary>
        public pMCROfFxDetector()
            : this(
            new DoubleRange(),
            new MultiLayerTissue(),
            new List<OpticalProperties>(),
            new List<int>(),
            true, // tallySecondMoment
            TallyType.pMCROfFx.ToString())
        {
        }
        /// <summary>
        /// detector mean
        /// </summary>
        [IgnoreDataMember]
        public Complex[] Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember]
        public Complex[] SecondMoment { get; set; }
        /// <summary>
        /// detector identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name, default uses TallyType, but can be user specified
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// number of time detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }
        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Fx { get; set; }

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
            var dp = photon.DP;

            var x = dp.Position.X;
            for (int ifx = 0; ifx < _fxArray.Length; ++ifx)
            {
                double freq = _fxArray[ifx];

                var sinNegativeTwoPiFX = Math.Sin(-2 * Math.PI * freq * x);
                var cosNegativeTwoPiFX = Math.Cos(-2 * Math.PI * freq * x);

                /* convert to Hz-sec from MHz-ns 1e-6*1e9=1e-3 */
                // convert to Hz-sec from GHz-ns 1e-9*1e9=1

                double weightFactor = _absorbAction(
                    photon.History.SubRegionInfoList.Select(c => c.NumberOfCollisions).ToList(),
                    photon.History.SubRegionInfoList.Select(p => p.PathLength).ToList(),
                    _perturbedOps);

                var deltaWeight = (weightFactor * dp.Weight) * (cosNegativeTwoPiFX + Complex.ImaginaryOne * sinNegativeTwoPiFX);

                Mean[ifx] += deltaWeight;
                if (_tallySecondMoment)
                {
                    var deltaWeight2 =
                        weightFactor * weightFactor * dp.Weight * dp.Weight * cosNegativeTwoPiFX * cosNegativeTwoPiFX +
                        weightFactor * weightFactor * Complex.ImaginaryOne * dp.Weight * dp.Weight * sinNegativeTwoPiFX * sinNegativeTwoPiFX;
                    // second moment of complex tally is square of real and imag separately
                    SecondMoment[ifx] += deltaWeight2;
                }
            }
            TallyCount++;
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
            for (int ifx = 0; ifx < Fx.Count; ifx++)
            {
                Mean[ifx] /= numPhotons;
                if (_tallySecondMoment)
                {
                    SecondMoment[ifx] /= numPhotons;
                }
            }
        }

        /// <summary>
        /// method to determine if photon within detector
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
