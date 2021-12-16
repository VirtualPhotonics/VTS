using System;
using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
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
                    return (previousDP, dp, regionIndex) => VolumeAbsorptionWeightingAnalog(dp);
                case AbsorptionWeightingType.Continuous:
                    if (detector.TallyType == TallyType.ATotal)
                    {
                        return (previousDP, dp, regionIndex) => VolumeAbsorptionWeightingContinuous(previousDP, dp);
                    }
                    else
                    {
                        throw new NotImplementedException("CAW is not currently implemented for most volume tallies.");
                    }
                case AbsorptionWeightingType.Discrete:
                    return (previousDP, dp, regionIndex) => VolumeAbsorptionWeightingDiscrete(previousDP, dp, regionIndex, tissue);
                default:
                    throw new ArgumentException("AbsorptionWeightingType did not match the available types.");
            }
        }
        /// <summary>
        /// Method that returns a function providing the correct absorption weighting for analog and DAW
        /// </summary>
        /// <param name="tissue">tissue specification</param>
        /// <param name="detector">detector specification</param>
        /// <returns>func providing correct absorption weighting for analog and DAW</returns>
        public static Func<IList<long>, IList<double>, IList<OpticalProperties>, IList<OpticalProperties>, IList<int>, double> GetpMCVolumeAbsorptionWeightingMethod(ITissue tissue, IDetector detector)
        {
            switch (tissue.AbsorptionWeightingType)
            {
                case AbsorptionWeightingType.Analog:
                    throw new NotImplementedException("Analog cannot be used for pMC estimates.");
                case AbsorptionWeightingType.Continuous:
                    if (detector.TallyType == TallyType.pMCATotal)
                    {
                        return (numberOfCollisions, pathLengths, perturbedOps, referenceOps,perturbedRegionIndices) => 
                            pMCVolumeAbsorptionWeightingContinuous(numberOfCollisions,pathLengths,perturbedOps,referenceOps,perturbedRegionIndices);
                    }
                    else
                    {
                        throw new NotImplementedException("CAW is not currently implemented for most volume tallies.");
                    }
                case AbsorptionWeightingType.Discrete:
                    if (detector.TallyType == TallyType.pMCATotal)
                    {
                        return (numberOfCollisions, pathLengths, perturbedOps, referenceOps, perturbedRegionIndices) =>
                            pMCVolumeAbsorptionWeightingDiscrete(numberOfCollisions, pathLengths, perturbedOps, referenceOps, perturbedRegionIndices);
                    }
                    else
                    {
                        throw new NotImplementedException("DAW is not currently implemented for most volume tallies.");
                    }
                default:
                    throw new ArgumentException("AbsorptionWeightingType did not match the available types.");
            }
        }
        /// <summary>
        /// Method that returns a function providing the correct absorption weighting for analog and DAW
        /// </summary>
        /// <param name="tissue">tissue specification</param>
        /// <param name="detector">detector specification</param>
        /// <returns>func providing correct absorption weighting for analog and DAW</returns>
        public static Func<IList<long>, IList<double>, IList<OpticalProperties>, IList<OpticalProperties>, IList<int>, double> GetpMCTerminationAbsorptionWeightingMethod(ITissue tissue, IDetector detector)
        {
            switch (tissue.AbsorptionWeightingType)
            {
                case AbsorptionWeightingType.Analog:
                    throw new NotImplementedException("Analog cannot be used for pMC estimates.");
                case AbsorptionWeightingType.Continuous:
                    return (numberOfCollisions, pathLength, perturbedOps, referenceOps, perturbedRegionsIndices) =>
                        pMCAbsorbContinuous(numberOfCollisions, pathLength, perturbedOps, referenceOps, perturbedRegionsIndices);
                case AbsorptionWeightingType.Discrete:
                    return (numberOfCollisions, pathLength, perturbedOps, referenceOps, perturbedRegionsIndices) =>
                        pMCAbsorbDiscrete(numberOfCollisions, pathLength, perturbedOps, referenceOps, perturbedRegionsIndices);
                default:
                    throw new ArgumentException("AbsorptionWeightingType did not match the available types.");
            }
        }

        private static double VolumeAbsorptionWeightingAnalog(PhotonDataPoint dp)
        {
            var weight = VolumeAbsorbAnalog(
                dp.StateFlag);

            return weight;
        }

        private static double VolumeAbsorptionWeightingDiscrete(PhotonDataPoint previousDP, PhotonDataPoint dp, int regionIndex, ITissue tissue)
        {
            var weight = VolumeAbsorbDiscrete(
                tissue.Regions[regionIndex].RegionOP.Mua,
                tissue.Regions[regionIndex].RegionOP.Mus,
                previousDP.Weight,
                dp.Weight);

            return weight;
        }
        private static double VolumeAbsorptionWeightingContinuous(
            PhotonDataPoint previousDP, PhotonDataPoint dp)
        {
            var weight = VolumeAbsorbContinuous(
                previousDP.Weight,
                dp.Weight);

            return weight;
        }

        private static double VolumeAbsorbAnalog(PhotonStateType photonStateType)
        {
            var weight = 0.0;
            if (photonStateType.HasFlag(PhotonStateType.Absorbed))
            {
                weight = 1.0;
            }
            return weight;
        }

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
        
        private static double VolumeAbsorbContinuous(double previousWeight, double weight)
        {
            // no pathlength means no absorption, so no tally
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
        
        private static double pMCAbsorbContinuous(IList<long> numberOfCollisions, IList<double> pathLength, IList<OpticalProperties> perturbedOps, IList<OpticalProperties> referenceOps, IList<int> perturbedRegionsIndices)
        {
            double weightFactor = 1.0;

            foreach (var i in perturbedRegionsIndices)
            {
                weightFactor *=
                    Math.Exp(-(perturbedOps[i].Mua - referenceOps[i].Mua) * pathLength[i]); // mua pert
                if ((numberOfCollisions[i] > 0) && (referenceOps[i].Mus > 0.0)) // mus pert
                {
                    // the following is more numerically stable
                    weightFactor *= Math.Pow(
                        (perturbedOps[i].Mus / referenceOps[i].Mus) * Math.Exp(-(perturbedOps[i].Mus - referenceOps[i].Mus) *
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

        private static double pMCAbsorbDiscrete(IList<long> numberOfCollisions, IList<double> pathLength, IList<OpticalProperties> perturbedOps, IList<OpticalProperties> referenceOps, IList<int> perturbedRegionsIndices)
        {
            double weightFactor = 1.0;

            foreach (var i in perturbedRegionsIndices)
            {
                if ((numberOfCollisions[i] > 0) && (referenceOps[i].Mus > 0.0))
                {
                    weightFactor *=
                        Math.Pow(
                          (perturbedOps[i].Mus / referenceOps[i].Mus) *
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

        private static double pMCVolumeAbsorptionWeightingDiscrete(IList<long> numberOfCollisions, IList<double> pathLengths, IList<OpticalProperties> perturbedOps, IList<OpticalProperties> referenceOps, IList<int> perturbedRegionsIndices)
        {
            // final pMC absorbed energy will use this perturbed factor 
            return pMCAbsorbDiscrete(numberOfCollisions, pathLengths, perturbedOps, referenceOps,
                perturbedRegionsIndices);
        }
        private static double pMCVolumeAbsorptionWeightingContinuous(IList<long> numberOfCollisions, IList<double> pathLengths, IList<OpticalProperties> perturbedOps, IList<OpticalProperties> referenceOps, IList<int> perturbedRegionsIndices)
        {
            // final pMC absorbed energy will use this perturbed factor 
            return pMCAbsorbContinuous(numberOfCollisions, pathLengths, perturbedOps, referenceOps,
                perturbedRegionsIndices);
        }
    }
}
