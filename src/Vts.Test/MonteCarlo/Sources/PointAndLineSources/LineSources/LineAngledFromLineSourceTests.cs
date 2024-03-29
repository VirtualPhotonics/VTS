﻿using MathNet.Numerics.Random;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for Line Angled From Line Sources
    /// </summary>
    [TestFixture]
    public class LineAngledFromLineSourceTests
    {
        /// <summary>
        /// test source input
        /// </summary>
        [Test]
        public void Validate_source_input_with_flat_profile_type()
        {
            // check default constructor
            var si = new LineAngledFromLineSourceInput();
            Assert.IsNotNull(si);
            // check full definition
            si = new LineAngledFromLineSourceInput(
                    10.0,
                    new FlatSourceProfile(),
                    SourceDefaults.DefaultPosition.Clone(),
                    1.0,
                    SourceDefaults.DefaultPosition.Clone(),
                    0
            );
            Assert.IsNotNull(si);
            // validate CreateSource
            var source = si.CreateSource(new MersenneTwister(0));
            Assert.IsNotNull(source);
        }
        /// <summary>
        /// Validate general constructor and implicitly validate GetFinalPosition
        /// and GetFinalDirection
        /// </summary>
        [Test]
        public void Validate_LineAngledFromLineSource_general_constructor()
        {
            var tissue = new MultiLayerTissue();
            var source = new LineAngledFromLineSource(
                10.0, // tissue line length
                new FlatSourceProfile(),
                new Position(0,0,0),
                1.0, // line in air length
                new Position(0, 0, -10), // center of line in air
                0);
            var photon = source.GetNextPhoton(tissue);
            // Position.X will be random between [-5 5] and Y and Z should be 0
            Assert.IsTrue(photon.DP.Position.X < 5);
            Assert.IsTrue(photon.DP.Position.X > -5);
            Assert.AreEqual(0.0, photon.DP.Position.Y);
            Assert.AreEqual(0.0, photon.DP.Position.Z);
            // Direction.Ux,Uz will be random but Uy should be 0
            Assert.AreEqual(0.0, photon.DP.Direction.Uy);
        }

    }
}
