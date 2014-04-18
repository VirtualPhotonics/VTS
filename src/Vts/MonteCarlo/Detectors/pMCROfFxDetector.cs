using System;
using System.Linq;
using MathNet.Numerics;
using System.Runtime.Serialization;
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
        private double[] _fxArray;
        private OpticalProperties[] _perturbedOps;
        private OpticalProperties[] _referenceOps;
        private int[] _perturbedRegionsIndices;
        private bool _tallySecondMoment;
        private Func<long[], double[], OpticalProperties[], OpticalProperties[], int[], double> _absorbAction;

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
            OpticalProperties[] perturbedOps,
            int[] perturbedRegionIndices,
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
            _referenceOps = tissue.Regions.Select(r => r.RegionOP).ToArray();
            _perturbedRegionsIndices = perturbedRegionIndices;
            _absorbAction = AbsorptionWeightingMethods.GetpMCTerminationAbsorptionWeightingMethod(tissue, this);
            TallyCount = 0;
        }

        /// <summary>
        /// Returns a default instance of pMCROfFxDetector (for serialization purposes only)
        /// </summary>
        public pMCROfFxDetector()
            : this(
            new DoubleRange(),
            new MultiLayerTissue(),
            new OpticalProperties[0],
            new int[0], 
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

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            var dp = photon.DP;

            var x = dp.Position.X;

            double weightFactor = _absorbAction(
                photon.History.SubRegionInfoList.Select(c => c.NumberOfCollisions).ToArray(),
                photon.History.SubRegionInfoList.Select(p => p.PathLength).ToArray(),
                _perturbedOps,
                _referenceOps,
                _perturbedRegionsIndices);

            for (int ifx = 0; ifx < _fxArray.Length; ++ifx)
            {
                double freq = _fxArray[ifx];

                var sinNegativeTwoPiFX = Math.Sin(-2 * Math.PI * freq * x);
                var cosNegativeTwoPiFX = Math.Cos(-2 * Math.PI * freq * x);

                /* convert to Hz-sec from MHz-ns 1e-6*1e9=1e-3 */
                // convert to Hz-sec from GHz-ns 1e-9*1e9=1

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
