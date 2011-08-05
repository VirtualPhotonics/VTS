using Vts.MonteCarlo;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Extensions
{
    public static class StatisticsExtensions
    {
        public static SimulationStatistics TrackDeathStatistics(this SimulationStatistics statistics, PhotonDataPoint dp)
        {
            if (dp.StateFlag.HasFlag(PhotonStateType.PseudoReflectedTissueBoundary))
            {
                ++statistics.NumberOfPhotonsOutTopOfTissue;
            }
            if (dp.StateFlag.HasFlag(PhotonStateType.PseudoTransmittedTissueBoundary))
            {
                ++statistics.NumberOfPhotonsOutBottomOfTissue;
            }
            if (dp.StateFlag.HasFlag(PhotonStateType.Absorbed))
            {
                ++statistics.NumberOfPhotonsAbsorbed;
            }
            if (dp.StateFlag.HasFlag(PhotonStateType.KilledOverMaximumCollisions))
            {
                ++statistics.NumberOfPhotonsKilledOverMaximumCollisions;
            }
            if (dp.StateFlag.HasFlag(PhotonStateType.KilledOverMaximumPathLength))
            {
                ++statistics.NumberOfPhotonsKilledOverMaximumPathLength;
            }
            if (dp.StateFlag.HasFlag(PhotonStateType.KilledRussianRoulette))
            {
                ++statistics.NumberOfPhotonsKilledByRussianRoulette;
            }
            return statistics;
        }
    }
}
