using Vts.MonteCarlo;

namespace Vts.MonteCarlo
{
    public static class StatisticsExtensions
    {
        public static void TrackStatistics(this SimulationStatistics statistics, PhotonHistory history)
        {
            if (history.HistoryData[history.HistoryData.Count - 1].StateFlag.
                Has(PhotonStateType.PseudoReflectedTissueBoundary))
            {
                ++statistics.NumberOfPhotonsOutTopOfTissue;
            }
            if (history.HistoryData[history.HistoryData.Count - 1].StateFlag.
                Has(PhotonStateType.PseudoTransmittedTissueBoundary))
            {
                ++statistics.NumberOfPhotonsOutBottomOfTissue;
            }
            if (history.HistoryData[history.HistoryData.Count - 1].StateFlag.
                Has(PhotonStateType.Absorbed))
            {
                ++statistics.NumberOfPhotonsAbsorbed;
            }
            if (history.HistoryData[history.HistoryData.Count - 1].StateFlag.
                Has(PhotonStateType.KilledOverMaximumCollisions))
            {
                ++statistics.NumberOfPhotonsKilledOverMaximumCollisions;
            }
            if (history.HistoryData[history.HistoryData.Count - 1].StateFlag.
                Has(PhotonStateType.KilledOverMaximumPathLength))
            {
                ++statistics.NumberOfPhotonsKilledOverMaximumPathLength;
            }
            if (history.HistoryData[history.HistoryData.Count - 1].StateFlag.
                Has(PhotonStateType.KilledRussianRoulette))
            {
                ++statistics.NumberOfPhotonsKilledByRussianRoulette;
            }
        }
    }
}
