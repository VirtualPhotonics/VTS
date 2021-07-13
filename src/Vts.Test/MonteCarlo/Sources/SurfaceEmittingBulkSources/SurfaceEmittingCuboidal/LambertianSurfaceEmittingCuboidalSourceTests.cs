using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for LambertianSurfaceEmittingCuboidalSource
    /// </summary>
    [TestFixture]
    public class LambertianSurfaceEmittingCuboidalSourceTests
    {
        /// <summary>
        /// This test validated using geometry assumptions
        /// </summary>
        [Test]
        public void Validate_starting_photons_off_surface_of_cuboidal()
        {
            Random rng =
                new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var cubeLengthX = 1.0;
            var cubeWidthY = 2.0;
            var cubeHeightZ = 2.0;
            var profile = new FlatSourceProfile();
            var directionAxis = new Direction(0, 0, 1);
            var translationFromOrigin = new Position(0, 0, 4);

            var ps = new LambertianSurfaceEmittingCuboidalSource(
                cubeLengthX,
                cubeWidthY,
                cubeHeightZ,
                profile,
                directionAxis,
                translationFromOrigin,
                0)
            {
                Rng = rng
            };
            // check 10 photons
            for (int i = 0; i < 10; i++)
            {
                var photon = ps.GetNextPhoton(tissue);
                // if on x is on 1 of 2 constant faces, make sure y and z are within boundaries
                if ((Math.Abs(photon.DP.Position.X + cubeLengthX / 2 + translationFromOrigin.X) < 1e-10) ||
                    (Math.Abs(photon.DP.Position.X - cubeLengthX / 2 + translationFromOrigin.X) < 1e-10))
                {
                    Assert.IsTrue((photon.DP.Position.Y < cubeWidthY / 2 + translationFromOrigin.Y) &&
                                  (photon.DP.Position.Y > -cubeWidthY / 2 + translationFromOrigin.Y));
                    Assert.IsTrue((photon.DP.Position.Z < cubeHeightZ / 2 + translationFromOrigin.Z) &&
                                  (photon.DP.Position.Z > -cubeHeightZ / 2 + translationFromOrigin.Z));
                }
                // if on y is on 1 of 2 constant faces, make sure x and z are within boundaries
                if ((Math.Abs(photon.DP.Position.Y + cubeWidthY / 2 + translationFromOrigin.Y) < 1e-10) ||
                    (Math.Abs(photon.DP.Position.Y - cubeWidthY / 2 + translationFromOrigin.Y) < 1e-10))
                {
                    Assert.IsTrue((photon.DP.Position.X < cubeLengthX / 2 + translationFromOrigin.X) &&
                                  (photon.DP.Position.X > -cubeLengthX / 2 + translationFromOrigin.X));
                    Assert.IsTrue((photon.DP.Position.Z < cubeHeightZ / 2 + translationFromOrigin.Z) &&
                                  (photon.DP.Position.Z > -cubeHeightZ / 2 + translationFromOrigin.Z));
                }
                // if on z is on 1 of 2 constant faces, make sure x and y are within boundaries
                if ((Math.Abs(photon.DP.Position.Z - cubeHeightZ / 2 - translationFromOrigin.Z) < 1e-10) ||
                    (Math.Abs(photon.DP.Position.Z + cubeHeightZ / 2 - translationFromOrigin.Z) < 1e-10))
                {
                    Assert.IsTrue((photon.DP.Position.X < cubeLengthX / 2 + translationFromOrigin.X) &&
                                  (photon.DP.Position.X > -cubeLengthX / 2 + translationFromOrigin.X));
                    Assert.IsTrue((photon.DP.Position.Y < cubeWidthY / 2 + translationFromOrigin.Y) &&
                                  (photon.DP.Position.Y > -cubeWidthY / 2 + translationFromOrigin.Y));
                }
            }
        }
    }
}
