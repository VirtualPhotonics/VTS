﻿using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for IsotropicVolumetricCuboidalSource
    /// </summary>
    [TestFixture]
    public class IsotropicVolumetricCuboidalSourceTests
    {
        /// <summary>
        /// This test validated using geometry assumptions
        /// </summary>
        [Test]
        public void Validate_starting_photons_inside_of_volume_cuboidal()
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

            var ps = new IsotropicVolumetricCuboidalSource(
                cubeLengthX,
                cubeWidthY,
                cubeHeightZ,
                profile,
                directionAxis,
                translationFromOrigin,
                1)
            {
                Rng = rng
            };
            // check 10 photons
            for (int i = 0; i < 10; i++)
            {
                var photon = ps.GetNextPhoton(tissue);
                Assert.IsTrue((photon.DP.Position.X < cubeLengthX / 2 + translationFromOrigin.X) &&
                              (photon.DP.Position.X > -cubeLengthX / 2 + translationFromOrigin.X));
                Assert.IsTrue((photon.DP.Position.Y < cubeWidthY / 2 + translationFromOrigin.Y) &&
                              (photon.DP.Position.Y > -cubeWidthY / 2 + translationFromOrigin.Y));
                Assert.IsTrue((photon.DP.Position.Z < cubeHeightZ / 2 + translationFromOrigin.Z) &&
                              (photon.DP.Position.Z > -cubeHeightZ / 2 + translationFromOrigin.Z));

            }
        }
    }
}