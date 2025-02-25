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
            // the following flags are mutually exclusive typically however cases of
            // both KilledByRussianRoulette and PseudoReflectedTissueBoundary have been
            // found so clear before testing.
            dp.StateFlag = dp.StateFlag.Add(PhotonStateType.PseudoReflectedTissueBoundary);
            statistics.TrackDeathStatistics(dp);
            Assert.That(statistics.NumberOfPhotonsOutTopOfTissue, Is.EqualTo(1));
            dp.StateFlag = dp.StateFlag.Remove(PhotonStateType.PseudoReflectedTissueBoundary);

            dp.StateFlag = dp.StateFlag.Add(PhotonStateType.PseudoTransmittedTissueBoundary);
            statistics.TrackDeathStatistics(dp);
            Assert.That(statistics.NumberOfPhotonsOutBottomOfTissue, Is.EqualTo(1));
            dp.StateFlag = dp.StateFlag.Remove(PhotonStateType.PseudoTransmittedTissueBoundary);

            dp.StateFlag = dp.StateFlag.Add(PhotonStateType.Absorbed);
            statistics.TrackDeathStatistics(dp);
            Assert.That(statistics.NumberOfPhotonsAbsorbed, Is.EqualTo(1));
            dp.StateFlag = dp.StateFlag.Remove(PhotonStateType.Absorbed);

            dp.StateFlag = dp.StateFlag.Add(PhotonStateType.PseudoSpecularTissueBoundary);
            statistics.TrackDeathStatistics(dp);
            Assert.That(statistics.NumberOfPhotonsSpecularReflected, Is.EqualTo(1));
            dp.StateFlag = dp.StateFlag.Remove(PhotonStateType.PseudoSpecularTissueBoundary);

            dp.StateFlag = dp.StateFlag.Add(PhotonStateType.KilledOverMaximumCollisions);
            statistics.TrackDeathStatistics(dp);
            Assert.That(statistics.NumberOfPhotonsKilledOverMaximumCollisions, Is.EqualTo(1));
            dp.StateFlag = dp.StateFlag.Remove(PhotonStateType.KilledOverMaximumCollisions);

            dp.StateFlag = dp.StateFlag.Add(PhotonStateType.KilledOverMaximumPathLength);
            statistics.TrackDeathStatistics(dp);
            Assert.That(statistics.NumberOfPhotonsKilledOverMaximumPathLength, Is.EqualTo(1));
            dp.StateFlag = dp.StateFlag.Remove(PhotonStateType.KilledOverMaximumPathLength);

            dp.StateFlag = dp.StateFlag.Add(PhotonStateType.KilledRussianRoulette);
            statistics.TrackDeathStatistics(dp);
            Assert.That(statistics.NumberOfPhotonsKilledByRussianRoulette, Is.EqualTo(1));
        }
    }
}

