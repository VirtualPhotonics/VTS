using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Extensions
{
    /// <summary>
    /// Methods to help track statistics in the Monte Carlo simulation
    /// </summary>
    public static class StatisticsExtensions
    {
        /// <summary>
        /// Method to determine statistics about how the photon died.
        /// </summary>
        /// <param name="statistics">SimulationStatistics class where statistics are kept</param>
        /// <param name="dp">PhotonDataPoint</param>
        /// <returns>Updated SimulationStatistics</returns>
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
            if (dp.StateFlag.HasFlag(PhotonStateType.PseudoSpecularTissueBoundary))
            {
                ++statistics.NumberOfPhotonsSpecularReflected;
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
