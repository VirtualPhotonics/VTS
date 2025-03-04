using MathNet.Numerics.Random;
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
            Assert.That(si, Is.Not.Null);
            // check full definition
            si = new LineAngledFromLineSourceInput(
                    10.0,
                    new FlatSourceProfile(),
                    SourceDefaults.DefaultPosition.Clone(),
                    1.0,
                    SourceDefaults.DefaultPosition.Clone(),
                    0
            );
            Assert.That(si, Is.Not.Null);
            // validate CreateSource
            var source = si.CreateSource(new MersenneTwister(0));
            Assert.That(source, Is.Not.Null);
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
            Assert.That(photon.DP.Position.X < 5, Is.True);
            Assert.That(photon.DP.Position.X > -5, Is.True);
            Assert.That(photon.DP.Position.Y, Is.EqualTo(0.0));
            Assert.That(photon.DP.Position.Z, Is.EqualTo(0.0));
            // Direction.Ux,Uz will be random but Uy should be 0
            Assert.That(photon.DP.Direction.Uy, Is.EqualTo(0.0));
        }

    }
}
