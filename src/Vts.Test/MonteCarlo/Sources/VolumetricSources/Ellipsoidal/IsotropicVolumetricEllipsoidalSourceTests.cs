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
    /// Unit tests for Surface Emitting Sources: IsotropicVolumetricEllipsoidalSources
    /// </summary>
    [TestFixture]
    public class IsotropicVolumetricEllipsoidalSourceTests
    {

        /// <summary>
        /// Validate General Constructor of Isotropic Flat VolumetricEllipsoidal Source
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
            var axisDirection = new Direction(0, 0, 1);
            var center = new Position(0, 0, 4);

            var ps = new IsotropicVolumetricEllipsoidalSource(
                aParameter, 
                bParameter,
                cParameter,
                profile,
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
                Assert.IsTrue(inside <= 1.0);
            }

        }

        /// <summary>
        /// Validate General Constructor of Isotropic Gaussian VolumetricEllipsoidal Source
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
            var axisDirection = new Direction(0, 0, 1);
            var center = new Position(0, 0, 4);

            var ps = new IsotropicVolumetricEllipsoidalSource(
                aParameter,
                bParameter,
                cParameter,
                profile,
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
                Assert.IsTrue(inside <= 1.0);
            }
        }

    }
}