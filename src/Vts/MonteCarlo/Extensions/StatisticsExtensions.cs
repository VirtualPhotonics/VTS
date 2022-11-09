using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Extensions
{
    /// <summary>
    /// Methods to help track statistics in the Monte Carlo simulation
    /// </summary>
    public static class StatisticsExtensions
    {
        /// <summary>
        /// Method to determine statistics about how the photon died.  These states
        /// are typically mutually exclusive, however there are occurrences of photons
        /// that are killed by Russian Roulette at start of track that crosses boundary
        /// and gets that state set as well but does not tally.
        /// It is for this reason the check for Russian Roulette is first.
        /// </summary>
        /// <param name="statistics">SimulationStatistics class where statistics are kept</param>
        /// <param name="dp">PhotonDataPoint</param>
        /// <returns>Updated SimulationStatistics</returns>
        public static SimulationStatistics TrackDeathStatistics(this SimulationStatistics statistics, PhotonDataPoint dp)
        {
            if (dp.StateFlag.HasFlag(PhotonStateType.KilledRussianRoulette))
            {
                ++statistics.NumberOfPhotonsKilledByRussianRoulette;
            }
            else if (dp.StateFlag.HasFlag(PhotonStateType.PseudoReflectedTissueBoundary))
            {
                ++statistics.NumberOfPhotonsOutTopOfTissue;
            }
            else if (dp.StateFlag.HasFlag(PhotonStateType.PseudoTransmittedTissueBoundary))
            {
                ++statistics.NumberOfPhotonsOutBottomOfTissue;
            }
            else if (dp.StateFlag.HasFlag(PhotonStateType.Absorbed))
            {
                ++statistics.NumberOfPhotonsAbsorbed;
            }
            else if (dp.StateFlag.HasFlag(PhotonStateType.PseudoSpecularTissueBoundary))
            {
                ++statistics.NumberOfPhotonsSpecularReflected;
            }
            else if (dp.StateFlag.HasFlag(PhotonStateType.KilledOverMaximumCollisions))
            {
                ++statistics.NumberOfPhotonsKilledOverMaximumCollisions;
            }
            else if (dp.StateFlag.HasFlag(PhotonStateType.KilledOverMaximumPathLength))
            {
                ++statistics.NumberOfPhotonsKilledOverMaximumPathLength;
            }

            return statistics;
        }
    }
}
