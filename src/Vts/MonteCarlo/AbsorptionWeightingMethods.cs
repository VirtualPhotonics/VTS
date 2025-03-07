using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// The <see cref="MonteCarlo"/> namespace contains the Monte Carlo classes for the transport of photons
    /// </summary>

    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }

    /// <summary>
    /// methods used to determine photon weight based on absorption weighting method
    /// </summary>
    public static class AbsorptionWeightingMethods
    {
        /// <summary>
        /// Method that returns a function providing the correct absorption weighting for analog, DAW and CAW ATotal
        /// </summary>
        /// <param name="tissue">tissue specification</param>
        /// <param name="detector">detector specification</param>
        /// <returns>func providing correct absorption weighting for analog and DAW</returns>
        public static Func<PhotonDataPoint, PhotonDataPoint, int, double> GetVolumeAbsorptionWeightingMethod(ITissue tissue, IDetector detector)
        {
            switch (tissue.AbsorptionWeightingType)
            {
                case AbsorptionWeightingType.Analog:
                    return (previousDp, dp, regionIndex) => VolumeAbsorptionWeightingAnalog(dp);
                case AbsorptionWeightingType.Continuous:
                    if (detector.TallyType == TallyType.ATotal)
                    {
                        return (previousDp, dp, regionIndex) => VolumeAbsorptionWeightingContinuous(previousDp, dp);
                    }

                    throw new NotImplementedException("CAW is not currently implemented for most volume tallies.");
                case AbsorptionWeightingType.Discrete:
                    return (previousDp, dp, regionIndex) => VolumeAbsorptionWeightingDiscrete(previousDp, dp, regionIndex, tissue);
                default:
                    throw new ArgumentException("AbsorptionWeightingType did not match the available types.");
            }
        }

        /// <summary>
        /// Method that returns a function providing the correct perturbation Monte Carlo absorption weighting
        /// for DAW and CAW for volume detectors.  pMC cannot be applied to Analog.
        /// </summary>
        /// <param name="tissue">tissue specification</param>
        /// <param name="detector">detector specification</param>
        /// <returns>func providing correct absorption weighting for analog and DAW</returns>
        public static Func<IList<long>, IList<double>, IList<OpticalProperties>, IList<OpticalProperties>, IList<int>, double> 
            GetpMCVolumeAbsorptionWeightingMethod(ITissue tissue, IDetector detector)
        {
            switch (tissue.AbsorptionWeightingType)
            {
                case AbsorptionWeightingType.Analog:
                    throw new NotImplementedException( "Analog cannot be used for pMC estimates.");
                case AbsorptionWeightingType.Continuous:
                    if (detector.TallyType == TallyType.pMCATotal)
                    {
                        return (numberOfCollisions, pathLengths, perturbedOps, referenceOps,perturbedRegionIndices) => 
                            PmcVolumeAbsorptionWeightingContinuous(numberOfCollisions,pathLengths,perturbedOps,referenceOps,perturbedRegionIndices);
                    }

                    throw new NotImplementedException("CAW is not currently implemented for most volume tallies.");
                case AbsorptionWeightingType.Discrete:
                    if (detector.TallyType == TallyType.pMCATotal)
                    {
                        return (numberOfCollisions, pathLengths, perturbedOps, referenceOps, perturbedRegionIndices) =>
                            PmcVolumeAbsorptionWeightingDiscrete(numberOfCollisions, pathLengths, perturbedOps, referenceOps, perturbedRegionIndices);
                    }

                    throw new NotImplementedException("DAW is not currently implemented for most volume tallies.");
                default:
                    throw new ArgumentException("AbsorptionWeightingType did not match the available types.");
            }
        }

        /// <summary>
        /// Method that returns a function providing the correct perturbation Monte Carlo absorption weighting for
        /// DAW and CAW for terminal detectors.  pMC cannot be applied to Analog.
        /// </summary>
        /// <param name="tissue">tissue specification</param>
        /// <param name="detector">detector specification</param>
        /// <returns>func providing correct absorption weighting for DAW and CAW, pMC cannot be applied to Analog.
        /// func (numberOfCollisions,path lengths,perturbedOps,referenceOPs,perturbedRegionIndices</returns>
        public static Func<IList<long>, IList<double>, IList<OpticalProperties>, IList<OpticalProperties>, IList<int>, double> 
            GetpMCTerminationAbsorptionWeightingMethod(ITissue tissue, IDetector detector)
        {
            return tissue.AbsorptionWeightingType switch
            {
                AbsorptionWeightingType.Analog => throw new NotImplementedException(
                    "Analog cannot be used for pMC estimates."),
                AbsorptionWeightingType.Continuous => (numberOfCollisions, pathLength, perturbedOps, referenceOps,
                    perturbedRegionsIndices) => PmcAbsorbContinuous(numberOfCollisions, pathLength, perturbedOps,
                    referenceOps, perturbedRegionsIndices),
                AbsorptionWeightingType.Discrete => (numberOfCollisions, pathLength, perturbedOps, referenceOps,
                    perturbedRegionsIndices) => PmcAbsorbDiscrete(numberOfCollisions, pathLength, perturbedOps,
                    referenceOps, perturbedRegionsIndices),
                _ => throw new ArgumentException("AbsorptionWeightingType did not match the available types.")
            };
        }

        /// <summary>
        /// Method that returns a function providing the correct differential Monte Carlo absorption weighting for
        /// DAW and CAW for terminal detectors.  dMC cannot be applied to Analog.
        /// </summary>
        /// <param name="tissue">tissue specification</param>
        /// <param name="detector">detector specification</param>
        /// <param name="derivativeType">Type of derivative, e.g. dMua or dMus</param>
        /// <returns>func providing correct absorption weighting for DAW and CAW, pMC cannot be applied to Analog</returns>
        /// func (numberOfCollisions,path lengths,perturbedOps,referenceOPs,perturbedRegionIndices</returns>
        public static Func<IList<long>, IList<double>, IList<OpticalProperties>, IList<OpticalProperties>, IList<int>, double>
            GetdMCTerminationAbsorptionWeightingMethod(ITissue tissue, IDetector detector, DifferentialMonteCarloType derivativeType)
        {
            switch (tissue.AbsorptionWeightingType)
            {
                case AbsorptionWeightingType.Analog:
                    throw new NotImplementedException(
                        "Analog cannot be used for dMC estimates.");
                case AbsorptionWeightingType.Continuous:
                case AbsorptionWeightingType.Discrete:
                    return derivativeType switch
                    {
                        DifferentialMonteCarloType.DMua => (numberOfCollisions, pathLengths, perturbedOps, referenceOps,
                            perturbedRegionIndices) => DmcDmuaAbsorbContinuousOrDiscrete(numberOfCollisions,
                            pathLengths, perturbedOps, referenceOps, perturbedRegionIndices),
                        DifferentialMonteCarloType.DMus => (numberOfCollisions, pathLengths, perturbedOps, referenceOps,
                            perturbedRegionIndices) => DmcDmusAbsorbContinuousOrDiscrete(numberOfCollisions,
                            pathLengths, perturbedOps, referenceOps, perturbedRegionIndices),
                        _ => throw new ArgumentException("Derivative Type did not match the available types.")
                    };
                default:
                    throw new ArgumentException("AbsorptionWeightingType did not match the available types.");
                    break;
            };
        }

        /// <summary>
        /// Method to determine weight due to absorption using Analog in volume tallies
        /// </summary>
        /// <param name="dp">photon data point</param>
        /// <returns>Analog weight</returns>
        private static double VolumeAbsorptionWeightingAnalog(PhotonDataPoint dp)
        {
            var weight = VolumeAbsorbAnalog(
                dp.StateFlag);

            return weight;
        }

        /// <summary>
        /// Method to determine weight due to absorption using Discrete Absorption Weighting (DAW) in volume tallies
        /// </summary>
        /// <param name="previousDp">previous data point</param>
        /// <param name="dp">current photon data point</param>
        /// <param name="regionIndex">tissue region index</param>
        /// <param name="tissue">ITIssue</param>
        /// <returns>DAW weight</returns>
        private static double VolumeAbsorptionWeightingDiscrete(PhotonDataPoint previousDp, PhotonDataPoint dp, int regionIndex, ITissue tissue)
        {
            var weight = VolumeAbsorbDiscrete(
                tissue.Regions[regionIndex].RegionOP.Mua,
                tissue.Regions[regionIndex].RegionOP.Mus,
                previousDp.Weight,
                dp.Weight);

            return weight;
        }

        /// <summary>
        /// Method to determine weight due to absorption using Continuous Absorption Weighting (CAW) in volume tallies
        /// </summary>
        /// <param name="previousDp">previous data point</param>
        /// <param name="dp">current photon data point</param>
        /// <returns>DAW weight</returns>
        private static double VolumeAbsorptionWeightingContinuous(
            PhotonDataPoint previousDp, PhotonDataPoint dp)
        {
            var weight = VolumeAbsorbContinuous(
                previousDp.Weight,
                dp.Weight);

            return weight;
        }

        /// <summary>
        /// This method returns the correct weight for absorption weighting in a volume
        /// using an Analog random walk process.
        /// </summary>
        /// <param name="photonStateType">Photon state type</param>
        /// <returns>Photon weight</returns>
        private static double VolumeAbsorbAnalog(PhotonStateType photonStateType)
        {
            return !photonStateType.HasFlag(PhotonStateType.Absorbed) ? 0.0 : 1.0;
        }

        /// <summary>
        /// This method returns the correct weight for absorption weighting in a volume
        /// using Discrete Absorption Weighting random walk process
        /// </summary>
        /// <param name="mua">Mua in volume</param>
        /// <param name="mus">Mus in volume</param>
        /// <param name="previousWeight">Weight of photon previous collision</param>
        /// <param name="weight">Weight of photon at current collision</param>
        /// <returns>Photon weight</returns>
        private static double VolumeAbsorbDiscrete(double mua, double mus, double previousWeight, double weight)
        {
            if (previousWeight == weight) // pseudo collision, so no tally
            {
                weight = 0.0;
            }
            else
            {
                weight = previousWeight * mua / (mua + mus);
            }
            return weight;
        }
        
        /// <summary>
        /// This method returns the correct weight for absorption weighting in a volume
        /// using Continuous Absorption Weighting random walk process
        /// </summary>
        /// <param name="previousWeight">Weight at prior collision</param>
        /// <param name="weight">Weight at current collision</param>
        /// <returns>Photon weight</returns>
        private static double VolumeAbsorbContinuous(double previousWeight, double weight)
        {
            // no path length means no absorption, so no tally
            if (previousWeight == weight)
            {
                weight = 0.0;
            }
            else
            {
                weight = previousWeight - weight;
            }
            return weight;
        }
        
        /// <summary>
        /// This method determines the perturbed weight using Continuous Absorption Weighting
        /// </summary>
        /// <param name="numberOfCollisions">Number of collision in perturbed region (j)</param>
        /// <param name="pathLength">Path length in perturbed region (S)</param>
        /// <param name="perturbedOps">Perturbed optical properties</param>
        /// <param name="referenceOps">Reference or background optical properties</param>
        /// <param name="perturbedRegionsIndices">Perturbed region tissue region indices</param>
        /// <returns>Photon weight</returns>
        private static double PmcAbsorbContinuous(IList<long> numberOfCollisions, IList<double> pathLength, IList<OpticalProperties> perturbedOps, IList<OpticalProperties> referenceOps, IList<int> perturbedRegionsIndices)
        {
            var weightFactor = 1.0;

            foreach (var i in perturbedRegionsIndices)
            {
                weightFactor *=
                    Math.Exp(-(perturbedOps[i].Mua - referenceOps[i].Mua) * pathLength[i]); // mua pert
                if (numberOfCollisions[i] > 0 && referenceOps[i].Mus > 0.0) // mus pert
                {
                    // the following is more numerically stable
                    weightFactor *= Math.Pow(
                        perturbedOps[i].Mus / referenceOps[i].Mus * Math.Exp(-(perturbedOps[i].Mus - referenceOps[i].Mus) *
                            pathLength[i] / numberOfCollisions[i]),
                        numberOfCollisions[i]);
                }
                else
                {
                    weightFactor *= Math.Exp(-(perturbedOps[i].Mus - referenceOps[i].Mus) * pathLength[i]);
                }
            }
            return weightFactor;
        }

        /// <summary>
        /// This method determines the perturbed weight using Discrete Absorption Weighting
        /// </summary>
        /// <param name="numberOfCollisions">Number of collision in perturbed region (j)</param>
        /// <param name="pathLength">Path length in perturbed region (S)</param>
        /// <param name="perturbedOps">Perturbed optical properties</param>
        /// <param name="referenceOps">Reference or background optical properties</param>
        /// <param name="perturbedRegionsIndices">Perturbed region tissue region indices</param>
        /// <returns>Photon weight</returns>
        private static double PmcAbsorbDiscrete(IList<long> numberOfCollisions, 
            IList<double> pathLength, IList<OpticalProperties> perturbedOps, IList<OpticalProperties> referenceOps, 
            IList<int> perturbedRegionsIndices)
        {
            var weightFactor = 1.0;

            foreach (var i in perturbedRegionsIndices)
            {
                if (numberOfCollisions[i] > 0 && referenceOps[i].Mus > 0.0)
                {
                    weightFactor *=
                        Math.Pow(
                          perturbedOps[i].Mus / referenceOps[i].Mus *
                          Math.Exp(-(perturbedOps[i].Mus + perturbedOps[i].Mua - referenceOps[i].Mus - referenceOps[i].Mua) *
                          pathLength[i] / numberOfCollisions[i]),
                              numberOfCollisions[i]);
                }
                else
                {
                    weightFactor *=
                        Math.Exp(-(perturbedOps[i].Mus + perturbedOps[i].Mua - referenceOps[i].Mus - referenceOps[i].Mua) *
                                pathLength[i]);
                }
            }
            return weightFactor;
        }

        /// <summary>
        /// Differential Monte Carlo estimate of derivative of terminal estimator with respect to Mua.
        /// This algorithm is correct for both DAW and CAW.
        /// </summary>
        /// <param name="numberOfCollisions">Number of collisions in perturbed region (j)</param>
        /// <param name="pathLength">Path length in perturbed region (S)</param>
        /// <param name="perturbedOps">Perturbed optical properties</param>
        /// <param name="referenceOps">Reference or background optical properties</param>
        /// <param name="perturbedRegionsIndices">Perturbed region tissue region indices</param>
        /// <returns>Photon weight</returns>
        private static double DmcDmuaAbsorbContinuousOrDiscrete(IList<long> numberOfCollisions, 
            IList<double> pathLength, IList<OpticalProperties> perturbedOps, IList<OpticalProperties> referenceOps, 
            IList<int> perturbedRegionsIndices)
        {
            var weightFactor = 1.0;
                
            // NOTE: following code only works for single perturbed region because derivative of
            // Radon-Nikodym product needs d(AB)=dA B + A dB and this does not produce that 
            // Check for only one perturbedRegionIndices specified by user performed in DataStructuresValidation
            var i = perturbedRegionsIndices[0];

            if (numberOfCollisions[i] <= 0 || !(referenceOps[i].Mus > 0.0)) return weightFactor; // mus pert

            // rearranged to be more numerically stable
            weightFactor *= -pathLength[i] *
                            Math.Pow(perturbedOps[i].Mus / referenceOps[i].Mus *
                                     Math.Exp(-(perturbedOps[i].Mus + perturbedOps[i].Mua -
                                                referenceOps[i].Mus - referenceOps[i].Mua) *
                                         pathLength[i] / numberOfCollisions[i]),
                                numberOfCollisions[i]);

            return weightFactor;
        }

        /// <summary>
        /// Differential Monte Carlo estimate of derivative of terminal estimator with respect to Mus.
        /// This algorithm is correct for both DAW and CAW.
        /// </summary>
        /// <param name="numberOfCollisions">Number of collisions in perturbed region (j)</param>
        /// <param name="pathLength">Path length in perturbed region (S)</param>
        /// <param name="perturbedOps">Perturbed optical properties</param>
        /// <param name="referenceOps">Reference or background optical properties</param>
        /// <param name="perturbedRegionsIndices">Perturbed region tissue region indices</param>
        /// <returns>Photon weight</returns>
        private static double DmcDmusAbsorbContinuousOrDiscrete(IList<long> numberOfCollisions, IList<double> pathLength, IList<OpticalProperties> perturbedOps, IList<OpticalProperties> referenceOps, IList<int> perturbedRegionsIndices)
        {
            var weightFactor = 1.0;

            // NOTE: following code only works for single perturbed region because derivative of
            // Radon-Nikodym product needs d(AB)=dA B + A dB and this does not produce that 
            // Check for only one perturbedRegionIndices specified by user performed in DataStructuresValidation
            var i = perturbedRegionsIndices[0];

            if (numberOfCollisions[i] <= 0 || !(referenceOps[i].Mus > 0.0) || !(perturbedOps[i].Mus > 0)) return weightFactor; // mus pert

            // rearranged to be more numerically stable
            weightFactor *= (numberOfCollisions[i] / perturbedOps[i].Mus - pathLength[i]) * // first factor
                            Math.Pow(perturbedOps[i].Mus / referenceOps[i].Mus *
                                     Math.Exp(-(perturbedOps[i].Mus + perturbedOps[i].Mua - 
                                                referenceOps[i].Mus - referenceOps[i].Mua) * 
                                         pathLength[i] / numberOfCollisions[i]), 
                                numberOfCollisions[i]);  // second factor  

            return weightFactor;
        }

        /// <summary>
        /// Perturbation Monte Carlo weight factor for Discrete Absorption Weighting in volume detectors
        /// </summary>
        /// <param name="numberOfCollisions">Number of collisions in perturbed region (j)</param>
        /// <param name="pathLengths">Path length in perturbed region (S)</param>
        /// <param name="perturbedOps">Perturbed optical properties</param>
        /// <param name="referenceOps">Reference or background optical properties</param>
        /// <param name="perturbedRegionsIndices">Perturbed region tissue region indices</param>
        /// <returns>Photon weight</returns>
        private static double PmcVolumeAbsorptionWeightingDiscrete(IList<long> numberOfCollisions, IList<double> pathLengths, IList<OpticalProperties> perturbedOps, IList<OpticalProperties> referenceOps, IList<int> perturbedRegionsIndices)
        {
            // final pMC absorbed energy will use this perturbed factor 
            return PmcAbsorbDiscrete(numberOfCollisions, pathLengths, perturbedOps, referenceOps,
                perturbedRegionsIndices);
        }

        /// <summary>
        /// Perturbation Monte Carlo weight factor for Continuous Absorption Weighting in volume detectors
        /// </summary>
        /// <param name="numberOfCollisions">Number of collisions in perturbed region (j)</param>
        /// <param name="pathLengths">Path length in perturbed region (S)</param>
        /// <param name="perturbedOps">Perturbed optical properties</param>
        /// <param name="referenceOps">Reference or background optical properties</param>
        /// <param name="perturbedRegionsIndices">Perturbed region tissue region indices</param>
        /// <returns>Photon weight</returns>
        private static double PmcVolumeAbsorptionWeightingContinuous(IList<long> numberOfCollisions, IList<double> pathLengths, IList<OpticalProperties> perturbedOps, IList<OpticalProperties> referenceOps, IList<int> perturbedRegionsIndices)
        {
            // final pMC absorbed energy will use this perturbed factor 
            return PmcAbsorbContinuous(numberOfCollisions, pathLengths, perturbedOps, referenceOps,
                perturbedRegionsIndices);
        }
    }
}
