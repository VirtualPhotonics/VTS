using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.PhotonData;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class PhotonHistoryTests
    {
        /// <summary>
        /// Validate constructor
        /// </summary>
        [Test]
        public void Validate_constructor_results()
        {
            // constructor
            var numberSubRegions = 3;
            var photonHistory = new PhotonHistory(numberSubRegions);
            Assert.That(photonHistory, Is.InstanceOf<PhotonHistory>());
        }
        /// <summary>
        /// Validate adding to history list
        /// </summary>
        [Test]
        public void Validate_adding_to_history_list()
        {
            // constructor
            var numberSubRegions = 3;
            var photonHistory = new PhotonHistory(numberSubRegions);
            var dp1 = new PhotonDataPoint(
                new Position(1, 2, 3),
                new Direction(0, 0, 1),
                1.0,
                0.0,
                PhotonStateType.Alive); 
            var dp2 = new PhotonDataPoint(
                new Position(4, 5, 6),
                new Direction(0, 0, 1),
                0.1,
                10.0,
                PhotonStateType.Absorbed);
            // before adding photon data points, check CurrentDP and PreviousDP are null
            Assert.That(photonHistory.CurrentDP, Is.Null);
            Assert.That(photonHistory.PreviousDP, Is.Null);
            photonHistory.AddDPToHistory(dp1);
            photonHistory.AddDPToHistory(dp2);
            // verify contents of HistoryData
            Assert.That(Math.Abs(photonHistory.HistoryData[0].Position.X - 1) < 1e-6, Is.True);
            Assert.That(Math.Abs(photonHistory.HistoryData[0].Position.Y - 2) < 1e-6, Is.True);
            Assert.That(Math.Abs(photonHistory.HistoryData[0].Position.Z - 3) < 1e-6, Is.True);
            Assert.That(Math.Abs(photonHistory.HistoryData[0].Weight - 1) < 1e-6, Is.True);
            Assert.That(photonHistory.HistoryData[0].StateFlag == PhotonStateType.Alive, Is.True);
            Assert.That(Math.Abs(photonHistory.HistoryData[1].Position.X - 4) < 1e-6, Is.True);
            Assert.That(Math.Abs(photonHistory.HistoryData[1].Position.Y - 5) < 1e-6, Is.True);
            Assert.That(Math.Abs(photonHistory.HistoryData[1].Position.Z - 6) < 1e-6, Is.True);
            Assert.That(Math.Abs(photonHistory.HistoryData[1].Weight - 0.1) < 1e-6, Is.True);
            Assert.That(photonHistory.HistoryData[1].StateFlag == PhotonStateType.Absorbed, Is.True);
            var currentDp = photonHistory.CurrentDP;
            Assert.That(currentDp.StateFlag == PhotonStateType.Absorbed, Is.True);
            var previousDp = photonHistory.PreviousDP;
            Assert.That(previousDp.StateFlag == PhotonStateType.Alive, Is.True);
        }

    }
}

