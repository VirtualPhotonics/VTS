using System;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    public static class AbsorptionWeightingMethods
    {
        /// <summary>
        /// Method that returns a function providing the correct absorption weighting for analog and DAW
        /// </summary>
        /// <param name="tissue"></param>
        /// <param name="detector"></param>
        /// <returns></returns>
        public static Func<PhotonDataPoint, PhotonDataPoint, int, double> GetVolumeAbsorptionWeightingMethod(ITissue tissue, IDetector detector)
        {
            switch (tissue.AbsorptionWeightingType)
            {
                case AbsorptionWeightingType.Analog:
                    return (previousDP, dp, regionIndex) => VolumeAbsorptionWeightingAnalog(dp);
                case AbsorptionWeightingType.Continuous:
                    throw new NotImplementedException("CAW is not currently implemented for volume tallies.");
                case AbsorptionWeightingType.Discrete:
                    return (previousDP, dp, regionIndex) => VolumeAbsorptionWeightingDiscrete(previousDP, dp, regionIndex, tissue);
                default:
                    throw new ArgumentException("AbsorptionWeightingType did not match the available types.");
            }
        }

        /// <summary>
        /// Method that returns a function providing the correct absorption weighting for analog and DAW
        /// </summary>
        /// <param name="tissue"></param>
        /// <param name="detector"></param>
        /// <returns></returns>
        public static Func<long[], double[], OpticalProperties[], OpticalProperties[], int[], double> GetpMCTerminationAbsorptionWeightingMethod(ITissue tissue, IDetector detector)
        {
            switch (tissue.AbsorptionWeightingType)
            {
                case AbsorptionWeightingType.Analog:
                    throw new NotImplementedException("CAW is not currently implemented for volume tallies.");
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
                dp.Weight,
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

        private static double VolumeAbsorbAnalog(double weight, PhotonStateType photonStateType)
        {
            if (photonStateType.HasFlag(PhotonStateType.Absorbed))
            {
                weight = 1.0;
            }
            else
            {
                weight = 0.0;
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

        private static double VolumeAbsorbContinuous(double mua, double mus, double previousWeight, double weight, PhotonStateType photonStateType, ITissue tissue, IDetector detector)
        {
            throw new NotImplementedException();
        }
        
        private static double pMCAbsorbContinuous(long[] numberOfCollisions, double[] pathLength, OpticalProperties[] perturbedOps, OpticalProperties[] referenceOps, int[] perturbedRegionsIndices)
        {
            double weightFactor = 1.0;

            foreach (var i in perturbedRegionsIndices)
            {
                weightFactor *=
                    Math.Exp(-(perturbedOps[i].Mua - referenceOps[i].Mua) * pathLength[i]); // mua pert
                if (numberOfCollisions[i] > 0) // mus pert
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

        private static double pMCAbsorbDiscrete(long[] numberOfCollisions, double[] pathLength, OpticalProperties[] perturbedOps, OpticalProperties[] referenceOps, int[] perturbedRegionsIndices)
        {
            double weightFactor = 1.0;

            foreach (var i in perturbedRegionsIndices)
            {
                if (numberOfCollisions[i] > 0)
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

        private static double pMCAbsorbAnalog(long[] numberOfCollisions, double[] pathLength, OpticalProperties[] perturbedOps, OpticalProperties[] referenceOps, int[] perturbedRegionsIndices)
        {
            throw new NotImplementedException();
        }
    }
}
