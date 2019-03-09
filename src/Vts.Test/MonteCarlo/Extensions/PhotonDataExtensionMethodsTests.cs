using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.PhotonData;

namespace Vts.Test.MonteCarlo.Extensions
{
    [TestFixture]
    public class PhotonDataExtensionMethodsTests
    {       
        /// <summary>
        /// Validate method IsWithinNA for fully open NA
        /// </summary>
        [Test]
        public void validate_IsWithinNA_returns_correct_value_when_fully_open()
        {
            var NA = 1.4;
            var detectorRegionN = 1.4;
            var direction = new Direction(0, 0, -1); // straight up
            var DP = new PhotonDataPoint(
                new Position(0, 0, 0),
                direction,
                1.0, // weight: not used
                1.0, // photon time of flight: note used
                PhotonStateType.Alive);
            bool result = DP.IsWithinNA(NA, Direction.AlongNegativeZAxis, detectorRegionN);
            Assert.AreEqual(result, true);
        }
        /// <summary>
        /// Validate IsWithinNA for partially open NA
        /// </summary>
        [Test]
        public void validate_IsWithinNA_returns_correct_value_when_partially_open()
        {
            var NA = 0.22;
            var detectorRegionN = 1.4;
            var direction = new Direction(0, 0, -1); // straight up
            var DP = new PhotonDataPoint(
                new Position(0, 0, 0),
                direction,
                1.0, // weight: not used
                1.0, // photon time of flight: note used
                PhotonStateType.Alive);
            bool result = DP.IsWithinNA(NA, Direction.AlongNegativeZAxis, detectorRegionN);
            Assert.AreEqual(result, true);
            // now select direction right on NA
            var theta = Math.Asin(NA / detectorRegionN);
            direction = new Direction(Math.Sin(theta), 0, -Math.Cos(theta)); // right on NA
            DP = new PhotonDataPoint(
                new Position(0, 0, 0),
                direction,
                1.0, // weight: not used
                1.0, // photon time of flight: note used
                PhotonStateType.Alive);
            result = DP.IsWithinNA(NA, Direction.AlongNegativeZAxis, detectorRegionN);
            Assert.AreEqual(result, true);
            // now select direction outside of NA
            theta = Math.Asin( (NA * (1.1)) / detectorRegionN);
            direction = new Direction(Math.Sin(theta), 0, -Math.Cos(theta)); 
            DP = new PhotonDataPoint(
                new Position(0, 0, 0),
                direction,
                1.0, // weight: not used
                1.0, // photon time of flight: note used
                PhotonStateType.Alive);
            result = DP.IsWithinNA(NA, Direction.AlongNegativeZAxis, detectorRegionN);
            Assert.AreEqual(result, false);
        }
    }
}

