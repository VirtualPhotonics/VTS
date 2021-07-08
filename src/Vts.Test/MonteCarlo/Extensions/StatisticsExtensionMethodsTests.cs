using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.PhotonData;

namespace Vts.Test.MonteCarlo.Extensions
{
    [TestFixture]
    public class StatisticsExtensionMethodsTests
    {
        /// <summary>
        /// Validate method TrackDeathStatistics 
        /// </summary>
        [Test]
        public void validate_TrackDeathStatistics_returns_correct_values()
        {
            var statistics = new SimulationStatistics();
            // set up PhotonDataPoint with appropriate StateFlag and verify
            var DP = new PhotonDataPoint(
                new Position(0, 0, 0),
                new Direction(0, 0, 1),
                1.0, // weight: not used
                1.0, // photon time of flight: note used
                PhotonStateType.Alive);
            DP.StateFlag = DP.StateFlag.Add(PhotonStateType.PseudoReflectedTissueBoundary);
            statistics.TrackDeathStatistics(DP);
            Assert.AreEqual(1, statistics.NumberOfPhotonsOutTopOfTissue);
            DP.StateFlag = DP.StateFlag.Add(PhotonStateType.PseudoTransmittedTissueBoundary);
            statistics.TrackDeathStatistics(DP);
            Assert.AreEqual(1, statistics.NumberOfPhotonsOutBottomOfTissue);
            DP.StateFlag = DP.StateFlag.Add(PhotonStateType.Absorbed);
            statistics.TrackDeathStatistics(DP);
            Assert.AreEqual(1, statistics.NumberOfPhotonsAbsorbed);
            DP.StateFlag = DP.StateFlag.Add(PhotonStateType.PseudoSpecularTissueBoundary);
            statistics.TrackDeathStatistics(DP);
            Assert.AreEqual(1, statistics.NumberOfPhotonsSpecularReflected);
            DP.StateFlag = DP.StateFlag.Add(PhotonStateType.KilledOverMaximumCollisions);
            statistics.TrackDeathStatistics(DP);
            Assert.AreEqual(1, statistics.NumberOfPhotonsKilledOverMaximumCollisions);
            DP.StateFlag = DP.StateFlag.Add(PhotonStateType.KilledOverMaximumPathLength);
            statistics.TrackDeathStatistics(DP);
            Assert.AreEqual(1, statistics.NumberOfPhotonsKilledOverMaximumPathLength);
            DP.StateFlag = DP.StateFlag.Add(PhotonStateType.KilledRussianRoulette);
            statistics.TrackDeathStatistics(DP);
            Assert.AreEqual(1, statistics.NumberOfPhotonsKilledByRussianRoulette);
        }
    }
}

