using System;
using System.Linq;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    public static class AbsorptionWeightingMethods
    {
        public static Func<PhotonDataPoint, PhotonDataPoint, int, double> GetAbsorptionWeightingMethod(ITissue tissue)
        {
            switch (tissue.AbsorptionWeightingType)
            {
                case AbsorptionWeightingType.Analog:
                    return (previousDP, dp, regionIndex) => VolumeAbsorptionWeightingAnalog(previousDP, dp, regionIndex, tissue);
                case AbsorptionWeightingType.Continuous:
                    throw new NotImplementedException("CAW is not currently implemented for volume tallies.");
                case AbsorptionWeightingType.Discrete:
                    return (previousDP, dp, regionIndex) => VolumeAbsorptionWeightingDiscrete(previousDP, dp, regionIndex, tissue);
                default:
                    throw new ArgumentException("AbsorptionWeightingType did not match the available types.");
            }
        }

        private static double VolumeAbsorptionWeightingAnalog(PhotonDataPoint previousDP, PhotonDataPoint dp, int regionIndex, ITissue tissue)
        {
            var weight = AbsorbAnalog(
                tissue.Regions[regionIndex].RegionOP.Mua,
                tissue.Regions[regionIndex].RegionOP.Mus,
                previousDP.Weight,
                dp.Weight,
                dp.StateFlag);

            return weight;
        }

        private static double VolumeAbsorptionWeightingDiscrete(PhotonDataPoint previousDP, PhotonDataPoint dp, int regionIndex, ITissue tissue)
        {
            var weight = AbsorbDiscrete(
                tissue.Regions[regionIndex].RegionOP.Mua,
                tissue.Regions[regionIndex].RegionOP.Mus,
                previousDP.Weight,
                dp.Weight,
                dp.StateFlag);

            return weight;
        }

        private static double AbsorbAnalog(double mua, double mus, double previousWeight, double weight, PhotonStateType photonStateType)
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

        private static double AbsorbDiscrete(double mua, double mus, double previousWeight, double weight, PhotonStateType photonStateType)
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

        private static double AbsorbContinuous(double mua, double mus, double previousWeight, double weight, PhotonStateType photonStateType)
        {
            throw new NotImplementedException();
        }
    }
}
