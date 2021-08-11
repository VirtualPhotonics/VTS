using System;
using MathNet.Numerics.Random;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for Surface Emitting Sources: CustomVolumetricEllipsoidalSources
    /// </summary>
    [TestFixture]
    public class CustomVolumetricEllipsoidalSourceTests
    {
        /// <summary>
        /// test source input
        /// </summary>
        [Test]
        public void validate_source_input_with_flat_profile_type()
        {
            // check default constructor
            var si = new CustomVolumetricEllipsoidalSourceInput();
            Assert.IsInstanceOf<CustomVolumetricEllipsoidalSourceInput>(si);
            // check full definition
            si = new CustomVolumetricEllipsoidalSourceInput(
                    1.0,
                    1.0,
                    2.0,
                    new FlatSourceProfile(),
                    SourceDefaults.DefaultFullPolarAngleRange.Clone(),
                    SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                    SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                    SourceDefaults.DefaultPosition.Clone(),
                    0
            );
            Assert.IsInstanceOf<CustomVolumetricEllipsoidalSourceInput>(si);
            // validate CreateSource
            var source = si.CreateSource(new MersenneTwister(0));
            Assert.IsInstanceOf<CustomVolumetricEllipsoidalSource>(source);
        }
        /// <summary>
        /// Validate General Constructor of Custom Flat VolumetricEllipsoidal Source
        /// </summary>
        [Test]
        public void Validate_general_constructor_with_flat_profiletype_for_custom_VolumetricEllipsoidal_source_test()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new FlatSourceProfile();
            var aParameter = 2.0;
            var bParameter = 2.0;
            var cParameter = 3.0;
            var polarRange = new DoubleRange(0, Math.PI, 2);
            var azimuthalRange = new DoubleRange(0, 2 * Math.PI, 2);
            var axisDirection = new Direction(0, 0, 1);
            var center = new Position(0, 0, 4);

            var ps = new CustomVolumetricEllipsoidalSource(
                aParameter, 
                bParameter,
                cParameter,
                profile,
                polarRange,
                azimuthalRange,
                axisDirection, 
                center, 
                1)
            {
                Rng = rng
            };
            // check 10 photons
            for (int i = 0; i < 10; i++)
            {
                var photon = ps.GetNextPhoton(tissue);
                var inside =
                    (photon.DP.Position.X - center.X) * (photon.DP.Position.X - center.X) / (aParameter * aParameter) +
                    (photon.DP.Position.Y - center.Y) * (photon.DP.Position.Y - center.Y) / (bParameter * bParameter) +
                    (photon.DP.Position.Z - center.Z) * (photon.DP.Position.Z - center.Z) / (cParameter * cParameter);
                // check position is inside volume
                Assert.IsTrue(inside <= 1.0);
            }

        }

        /// <summary>
        /// Validate General Constructor of Custom Gaussian VolumetricEllipsoidal Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_gaussian_profiletype_for_custom_VolumetricEllipsoidal_source_test()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new GaussianSourceProfile(1.0);
            var aParameter = 2.0;
            var bParameter = 2.0;
            var cParameter = 3.0;
            var polarRange = new DoubleRange(0, Math.PI, 2);
            var azimuthalRange = new DoubleRange(0, 2 * Math.PI, 2);
            var axisDirection = new Direction(0, 0, 1);
            var center = new Position(0, 0, 4);

            var ps = new CustomVolumetricEllipsoidalSource(
                aParameter,
                bParameter,
                cParameter,
                profile,
                polarRange,
                azimuthalRange,
                axisDirection,
                center,
                1)
            {
                Rng = rng
            };
            // check 10 photons
            for (int i = 0; i < 10; i++)
            {
                var photon = ps.GetNextPhoton(tissue);
                var inside = 
                    (photon.DP.Position.X - center.X) / aParameter + (photon.DP.Position.X - center.X) / aParameter +
                    (photon.DP.Position.Y - center.Y) / bParameter + (photon.DP.Position.Y - center.Y) / bParameter +
                    (photon.DP.Position.Z - center.Z) / cParameter + (photon.DP.Position.Z - center.Z) / cParameter;
                // check position is inside volume
                Assert.IsTrue(inside <= 1.0);
            }
        }

    }
}