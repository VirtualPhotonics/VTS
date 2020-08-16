using System;
using System.IO;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for Adjoint Line Sources
    /// </summary>
    [TestFixture]
    public class AdjointLineSourcesTests
    {
        /// <summary>
        /// Validate general contructor and implicitly validate GetFinalPosition
        /// and GetFinalDirection
        /// </summary>
        [Test]
        public void validate_AdjointLineSource_general_constructor()
        {
            var tissue = new MultiLayerTissue();
            var source = new AdjointLineSource(1.0, 10.0, new Position(0, 0, -10), 0);
            var photon = source.GetNextPhoton(tissue);
            // Position.X will be random between [-5 5] and Y and Z should be 0
            Assert.IsTrue(photon.DP.Position.X < 5);
            Assert.IsTrue(photon.DP.Position.X > -5);
            Assert.AreEqual(photon.DP.Position.Y, 0.0);
            Assert.AreEqual(photon.DP.Position.Z, 0.0);
            // Direction.Ux,Uz will be random but Uy should be 0
            Assert.AreEqual(photon.DP.Direction.Uy, 0.0);
        }

    }
}
