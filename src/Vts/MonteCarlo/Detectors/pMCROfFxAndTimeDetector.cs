using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements IDetector&lt;Complex[,]&gt;. Tally for pMC estimation of reflectance 
    /// as a function of Fx and Time. Perturbations of just mua or mus alone are also
    /// handled by this class.
    /// </summary>
    [KnownType(typeof(pMCROfFxAndTimeDetector))]
    public class pMCROfFxAndTimeDetector : IDetector<Complex[,]>
    {
        private double[] _fxArray;
        private OpticalProperties[] _perturbedOps;
        private OpticalProperties[] _referenceOps;
        private int[] _perturbedRegionsIndices;
        private bool _tallySecondMoment;
        private Func<long[], double[], OpticalProperties[], OpticalProperties[], int[], double> _absorbAction;

        /// <summary>
        /// Returns an instance of pMCROfFxAndTimeDetector. Tallies perturbed R(fx,time). Instantiate with reference optical properties. When
        /// method Tally invoked, perturbed optical properties passed.
        /// </summary>
        /// <param name="fx">fx binning</param>
        /// <param name="time">time binning</param>
        /// <param name="tissue">tissue definition</param>
        /// <param name="perturbedOps">list of perturbed optical properties, indexing matches tissue indexing</param>
        /// <param name="perturbedRegionIndices">list of perturbed tissue region indices, indexing matches tissue indexing</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment info for error results</param>
        /// <param name="name">detector name</param>
        public pMCROfFxAndTimeDetector(
            DoubleRange fx,
            DoubleRange time,
            ITissue tissue,
            OpticalProperties[] perturbedOps,
            int[] perturbedRegionIndices,
            bool tallySecondMoment,
            String name)
        {
            Fx = fx;
            _fxArray = fx.AsEnumerable().ToArray();
            Time = time;
            _tallySecondMoment = tallySecondMoment;
            Mean = new Complex[Fx.Count - 1, Time.Count - 1];
            SecondMoment = null;
            if (_tallySecondMoment)
            {
                SecondMoment = new Complex[Fx.Count - 1, Time.Count - 1];
            }
            TallyType = TallyType.pMCROfFxAndTime;
            Name = name;
            _referenceOps = tissue.Regions.Select(r => r.RegionOP).ToArray();
            _perturbedOps = perturbedOps;
            _perturbedRegionsIndices = perturbedRegionIndices;
            _absorbAction = AbsorptionWeightingMethods.GetpMCTerminationAbsorptionWeightingMethod(tissue, this);
            TallyCount = 0;
        }

        /// <summary>
        /// Returns a default instance of pMCMuaMusROfFxAndTimeDetector (for serialization purposes only)
        /// </summary>
        public pMCROfFxAndTimeDetector()
            : this(
            new DoubleRange(),
            new DoubleRange(),
            new MultiLayerTissue(),
            new OpticalProperties[0],
            new int[0],
            true, // tallySecondMoment
            TallyType.pMCROfFxAndTime.ToString())
        {
        }

        /// <summary>
        /// mean of detector tally
        /// </summary>
        [IgnoreDataMember]
        public Complex[,] Mean { get; set; }

        /// <summary>
        /// second moment of detector tally
        /// </summary>
        [IgnoreDataMember]
        public Complex[,] SecondMoment { get; set; }

        /// <summary>
        /// detector identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name, default uses TallyType, but can be user specified
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// number of times detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }
        /// <summary>
        /// fx binning
        /// </summary>
        public DoubleRange Fx { get; set; }
        /// <summary>
        /// time binning
        /// </summary>
        public DoubleRange Time { get; set; }
        
        /// <summary>
        /// Method to tally to detector using information in Photon
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            var dp = photon.DP;

            var x = dp.Position.X;
            var it = DetectorBinning.WhichBinExclusive(dp.TotalTime, Time.Count - 1, Time.Delta, Time.Start);
            
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

                Mean[ifx, it] += deltaWeight;
                if (_tallySecondMoment)
                {
                    var deltaWeight2 =
                        weightFactor * weightFactor * dp.Weight * dp.Weight * cosNegativeTwoPiFX * cosNegativeTwoPiFX +
                        weightFactor * weightFactor * Complex.ImaginaryOne * dp.Weight * dp.Weight * sinNegativeTwoPiFX * sinNegativeTwoPiFX;
                    // second moment of complex tally is square of real and imag separately
                    SecondMoment[ifx, it] += deltaWeight2;
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

        /// <summary>
        /// Method to normalize the tally to get Mean and Second Moment estimates
        /// </summary>
        /// <param name="numPhotons">Number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2 * Math.PI * Fx.Delta * Time.Delta;
            for (int ir = 0; ir < Fx.Count - 1; ir++)
            {
                for (int it = 0; it < Time.Count - 1; it++)
                {
                    var areaNorm = (Fx.Start + (ir + 0.5) * Fx.Delta) * normalizationFactor;
                    Mean[ir, it] /= areaNorm * numPhotons;
                    // the above is pi(rmax*rmax-rmin*rmin) * timeDelta * N
                    if (_tallySecondMoment)
                    {
                        SecondMoment[ir, it] /= areaNorm * areaNorm * numPhotons;
                    }
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
            //return (dp.StateFlag.Has(PhotonStateType.PseudoTransmissionDomainTopBoundary));
        }
    }
}
