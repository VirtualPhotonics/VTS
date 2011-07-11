using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Helpers;

namespace Vts.Test.MonteCarlo.Sources
{
    [TestFixture]
    public class CustomLineSourceTests
    {
        [Test]
        public void validate_general_constructor_with_flat_profiletype_getnextphoton_assigns_correct_values1()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue(); // todo: remove

            var position = new Position(1.0, 2.0, 3.0);
            var direction = new Direction(1.0, 0.0, 0.0);
            var beamrot = new PolarAzimuthalAngles(0.25 * Math.PI, 0.25 * Math.PI);
            var polRange = new DoubleRange(0.0, 0.5 * Math.PI);
            var aziRange = new DoubleRange(0.0, Math.PI);
            var length = 1.0;
            var profile = new FlatSourceProfile();


            var ps = new CustomLineSource(length, profile, polRange, aziRange, direction, position, beamrot)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - 0.775510086631760), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - 0.488676493478267), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - 0.399724142696167), 0.0000000001);

            Assert.Less(Math.Abs(photon.DP.Position.X - 1.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Y - 2.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Z - 2.951186497567964), 0.0000000001);
        }

        [Test]
        public void validate_general_constructor_with_flat_profiletype_getnextphoton_assigns_correct_values2()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue(); // todo: remove

            var position = new Position(1.0, 2.0, 3.0);
            var direction = new Direction(1.0, 0.0, 0.0);
            var polRange = new DoubleRange(0.0, 0.5 * Math.PI);
            var aziRange = new DoubleRange(0.0, Math.PI);
            var length = 1.0;
            var profile = new FlatSourceProfile();


            var ps = new CustomLineSource(length, profile, polRange, aziRange, direction, position)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - 0.592844616526934), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - 0.628194114249385), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - 0.503892265744834), 0.0000000001);

            Assert.Less(Math.Abs(photon.DP.Position.X - 1.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Y - 2.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Z - 2.951186497567964), 0.0000000001);
        }

        [Test]
        public void validate_general_constructor_with_gaussian_profiletype_getnextphoton_assigns_correct_values()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue(); // todo: remove

            var position = new Position(1.0, 2.0, 3.0);
            var direction = new Direction(1.0, 0.0, 0.0);
            var beamrot = new PolarAzimuthalAngles(0.25 * Math.PI, 0.25 * Math.PI);
            var polRange = new DoubleRange(0.0, 0.5 * Math.PI);
            var aziRange = new DoubleRange(0.0, Math.PI);
            var length = 1.0;
            var beamFWHM = 0.8;
            var profile = new GaussianSourceProfile(beamFWHM);


            var ps = new CustomLineSource(length, profile, polRange, aziRange, direction, position, beamrot)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - 0.941954101584871), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - 0.281393782442861), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - 0.183139317760928), 0.0000000001);

            Assert.Less(Math.Abs(photon.DP.Position.X - 1.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Y - 2.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Z - 3.424771649138186), 0.0000000001);
        }        
    }
}
