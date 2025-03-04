using System;
using MathNet.Numerics.Random;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for LambertianSurfaceEmittingCylindricalFiberSource
    /// </summary>
    [TestFixture]
    public class LambertianSurfaceEmittingCylindricalFiberSourceTests
    {
        /// <summary>
        /// test source input
        /// </summary>
        [Test]
        public void Validate_source_input_with_flat_profile_type()
        {
            // check default constructor
            var si = new LambertianSurfaceEmittingCylindricalFiberSourceInput();
            Assert.That(si, Is.Not.Null);
            // check full definition
            si = new LambertianSurfaceEmittingCylindricalFiberSourceInput(
                    1.0,
                    1.0,
                    1.0,
                    1.0,
                    SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                    SourceDefaults.DefaultPosition.Clone(),
                    0
            );
            Assert.That(si, Is.Not.Null);
            // validate CreateSource
            var source = si.CreateSource(new MersenneTwister(0));
            Assert.That(source, Is.Not.Null);
        }
        /// <summary>
        /// This test validated using geometry assumptions
        /// </summary>
        [Test]
        public void Validate_starting_photons_off_surface_of_fiber()
        {
            Random rng =
                new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue(); 
            var fiberRadius = 1.0;
            var fiberHeight = 4;
            var curvedSurfaceEfficiency = 0.5;
            var bottomSurfaceEfficiency = 1.0;
            var principalAxis = new Direction(0, 0, 1);
            var translationFromOrigin = new Position(0, 0, 2);

            var ps = new LambertianSurfaceEmittingCylindricalFiberSource(
                fiberRadius,
                fiberHeight,
                curvedSurfaceEfficiency,
                bottomSurfaceEfficiency,
                principalAxis,
                translationFromOrigin,
                1)
            {
                Rng = rng
            };
            var photon = ps.GetNextPhoton(tissue);
            // make sure initial x-y position is on surface if not on bottom
            if (Math.Abs(photon.DP.Position.Z - 4) > 1e-10 ) // on sides
            {
                Assert.That(Math.Abs(fiberRadius -
                                     Math.Sqrt(photon.DP.Position.X * photon.DP.Position.X + 
                                               photon.DP.Position.Y * photon.DP.Position.Y)), Is.LessThan(0.00001));
            }
            else // on bottom, make sure pointed down
            {
                Assert.That(photon.DP.Direction.Uz > 0, Is.True);
            }
        }
    }
}
