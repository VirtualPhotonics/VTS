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
        public void Validate_TrackDeathStatistics_returns_correct_values()
        {
            var statistics = new SimulationStatistics();
            // set up PhotonDataPoint with appropriate StateFlag and verify
            var dp = new PhotonDataPoint(
                new Position(0, 0, 0),
                new Direction(0, 0, 1),
                1.0, // weight: not used
                1.0, // photon time of flight: note used
                PhotonStateType.Alive);
            dp.StateFlag = dp.StateFlag.Add(PhotonStateType.PseudoReflectedTissueBoundary);
            statistics.TrackDeathStatistics(dp);
            Assert.AreEqual(1, statistics.NumberOfPhotonsOutTopOfTissue);
            dp.StateFlag = dp.StateFlag.Add(PhotonStateType.PseudoTransmittedTissueBoundary);
            statistics.TrackDeathStatistics(dp);
            Assert.AreEqual(1, statistics.NumberOfPhotonsOutBottomOfTissue);
            dp.StateFlag = dp.StateFlag.Add(PhotonStateType.Absorbed);
            statistics.TrackDeathStatistics(dp);
            Assert.AreEqual(1, statistics.NumberOfPhotonsAbsorbed);
            dp.StateFlag = dp.StateFlag.Add(PhotonStateType.PseudoSpecularTissueBoundary);
            statistics.TrackDeathStatistics(dp);
            Assert.AreEqual(1, statistics.NumberOfPhotonsSpecularReflected);
            dp.StateFlag = dp.StateFlag.Add(PhotonStateType.KilledOverMaximumCollisions);
            statistics.TrackDeathStatistics(dp);
            Assert.AreEqual(1, statistics.NumberOfPhotonsKilledOverMaximumCollisions);
            dp.StateFlag = dp.StateFlag.Add(PhotonStateType.KilledOverMaximumPathLength);
            statistics.TrackDeathStatistics(dp);
            Assert.AreEqual(1, statistics.NumberOfPhotonsKilledOverMaximumPathLength);
            dp.StateFlag = dp.StateFlag.Add(PhotonStateType.KilledRussianRoulette);
            statistics.TrackDeathStatistics(dp);
            Assert.AreEqual(1, statistics.NumberOfPhotonsKilledByRussianRoulette);
        }
    }
}

