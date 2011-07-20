using Vts.MonteCarlo;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Extensions
{
    public static class StatisticsExtensions
    {
        public static SimulationStatistics TrackDeathStatistics(this SimulationStatistics statistics, PhotonDataPoint dp)
        {
            if (dp.StateFlag.Has(PhotonStateType.PseudoReflectedTissueBoundary))
            {
                ++statistics.NumberOfPhotonsOutTopOfTissue;
            }
            if (dp.StateFlag.Has(PhotonStateType.PseudoTransmittedTissueBoundary))
            {
                ++statistics.NumberOfPhotonsOutBottomOfTissue;
            }
            if (dp.StateFlag.Has(PhotonStateType.Absorbed))
            {
                ++statistics.NumberOfPhotonsAbsorbed;
            }
            if (dp.StateFlag.Has(PhotonStateType.KilledOverMaximumCollisions))
            {
                ++statistics.NumberOfPhotonsKilledOverMaximumCollisions;
            }
            if (dp.StateFlag.Has(PhotonStateType.KilledOverMaximumPathLength))
            {
                ++statistics.NumberOfPhotonsKilledOverMaximumPathLength;
            }
            if (dp.StateFlag.Has(PhotonStateType.KilledRussianRoulette))
            {
                ++statistics.NumberOfPhotonsKilledByRussianRoulette;
            }
            return statistics;
        }
    }
}
