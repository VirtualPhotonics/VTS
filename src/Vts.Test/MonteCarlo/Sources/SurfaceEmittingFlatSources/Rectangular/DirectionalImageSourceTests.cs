using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for Surface Emitting Sources: DirectionalImageSources
    /// Note: the test image in Resources, "circle.png" was created by using
    /// Inkscape and saving to a .csv file.
    /// </summary>
    [TestFixture]
    public class DirectionalImageSourceTests
    {
        DirectionalImageSourceInput _sourceInput;

        [OneTimeSetUp]
        public void Setup()
        {
            // read in image file locally
            const string folder = "";
            var name = Assembly.GetExecutingAssembly().FullName;
            if (name == null) return; 
            var assemblyName = new AssemblyName(name).Name;
            FileIO.CopyFileFromEmbeddedResources(
                assemblyName + ".Resources.sourcetest.circle.csv", 
                Path.Combine(folder, "circle.csv"), 
                name);
            // set up infile
            _sourceInput = new DirectionalImageSourceInput(
                "",
                "circle.csv",
                113,
                102,
                0.1,
                0.1,
                0.0, // normal incidence
                new Direction(0, 0, 1), // principal source axis
                new Position(0, 0, 0), // no translation
                new PolarAzimuthalAngles(),
                0);
        }

        /// <summary>
        /// Use infile in setup to instantiate source class and validate settings
        /// </summary>
        [Test]
        public void Validate_construction_of_source_class()
        {
            // verify property settings
            var source = new DirectionalImageSourceInput(
                _sourceInput.InputFolder,
                _sourceInput.ImageName,
                _sourceInput.NumberOfPixelsX,
                _sourceInput.NumberOfPixelsY,
                _sourceInput.PixelWidthX,
                _sourceInput.PixelHeightY);
            Assert.AreEqual(113, source.NumberOfPixelsX);
            Assert.AreEqual(102, source.NumberOfPixelsY);
            Assert.AreEqual(0.1, source.PixelWidthX);
            Assert.AreEqual(0.1, source.PixelHeightY);

            // call CreateSource and verify settings
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); 
            source.CreateSource(rng);
            // verify X and Y properties
            Assert.IsTrue(Math.Abs(-5.65 - ((ImageSourceProfile)source.SourceProfile).X.Start) < 1e-6 );
            Assert.IsTrue(Math.Abs(5.65 -  ((ImageSourceProfile)source.SourceProfile).X.Stop) < 1e-6);
            Assert.AreEqual(114, ((ImageSourceProfile)source.SourceProfile).X.Count);
            Assert.IsTrue(Math.Abs(-5.1 - ((ImageSourceProfile)source.SourceProfile).Y.Start) < 1e-6);
            Assert.IsTrue(Math.Abs(5.1 - ((ImageSourceProfile)source.SourceProfile).Y.Stop) < 1e-6);
            Assert.AreEqual(103, ((ImageSourceProfile)source.SourceProfile).Y.Count);
            // verify Image property
            Assert.IsTrue(Math.Abs(1 - ((ImageSourceProfile)source.SourceProfile).Image[539]) < 1e-6);
        }

        /// <summary>
        /// Run infile in setup and validate starting locations of photons.
        /// Transport photons through a transparent tissue so that source
        /// specification should be directly T(x,y)
        /// </summary>
        [Test]
        public void Validate_starting_photon_locations_based_on_image()
        {
            var simulationOptions = new SimulationOptions { Seed = 0 };
            var tissueInput = new MultiLayerTissueInput(
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 1.0), // thin transparent layer
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(1.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                }
            );
            var detectorInputs = new List<IDetectorInput>
            {
                new TOfXAndYDetectorInput
                {
                    X = new DoubleRange(-15, 15, 31),
                    Y = new DoubleRange(-15, 15, 31)
                }
            };
            var input = new SimulationInput(
                100,
                "",
                simulationOptions,
                _sourceInput,
                tissueInput,
                detectorInputs);
            var output = new MonteCarloSimulation(input).Run();
            // validate T(x,y)
            Assert.IsTrue(output.T_xy[14, 15] > 0.0); // center of image
            Assert.IsTrue(Math.Abs(output.T_xy[0, 0] - 0.0) < 1e-6); // corner of image
        }

        /// <summary>
        /// Instantiate ImageSourceProfile and validate GetBinaryPixelMap 
        /// and GetPositionInARectangleBasedOnImageIntensity methods
        /// </summary>
        [Test]
        public void Validate_ImageSourceProfile_general_constructor_and_methods_test()
        {
            Random rng =
                new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            // make a intensity varied source
            var image = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }; // representing a 3x3 pixel image
            var arbitrarySourceProfile = new ImageSourceProfile(
                image,
                3,
                3,
                1,
                1,
                new Vts.Common.Position(0, 0, 0));
            // verify X, Y 
            Assert.IsTrue(Math.Abs(-1.5 - arbitrarySourceProfile.X.Start) < 1e-6);
            Assert.IsTrue(Math.Abs(1.5 - arbitrarySourceProfile.X.Stop) < 1e-6);
            Assert.AreEqual(4, arbitrarySourceProfile.X.Count);
            Assert.IsTrue(Math.Abs(-1.5 - arbitrarySourceProfile.Y.Start) < 1e-6);
            Assert.IsTrue(Math.Abs(1.5 - arbitrarySourceProfile.Y.Stop) < 1e-6);
            Assert.AreEqual(4, arbitrarySourceProfile.Y.Count);

            // verify binary bit map
            var binaryMap = arbitrarySourceProfile.GetBinaryPixelMap();
            Assert.IsTrue(binaryMap[0] == 0);
            for (var i = 1; i < image.Length; i++)
            {
                Assert.IsTrue(binaryMap[i] == 1);
            }

            // verify GetPositionInARectangleBasedOnImageIntensity method for 10 calls 
            for (var i = 0; i < 10; i++)
            {
                var r = arbitrarySourceProfile.GetPositionInARectangleBasedOnImageIntensity(rng);
                if (r == null) continue;
                Assert.IsTrue(Math.Abs(r.Z) < 1e-6); // z is always 0
                Assert.IsTrue(Math.Abs(r.X) <= 1.5); // x is in [-1.5, 1.5]
                Assert.IsTrue(Math.Abs(r.Y) <= 1.5); // y is in [-1.5, 1.5]
            }

        }
        /// <summary>
        /// Create ImageSourceProfile with all intensity in one pixel,
        /// then use this object in general constructor of DirectionalImageSource
        /// and validate initiated new Photon
        /// </summary>
        [Test]
        public void Validate_construction_of_photons_given_specified_source_profile_test()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            // make a intensity varied source
            var image = new double[] { 0, 1, 0, 0, 0, 0, 0, 0, 0}; // representing a 3x3 pixel image
            var arbitrarySourceProfile = new ImageSourceProfile(
                image,
                3,
                3,
                1,
                1,
                new Vts.Common.Position(0, 0, 0));

            // instantiate source with profile
            var ps = new DirectionalImageSource(
                0.0, // normal
                3.0,
                3.0,
                arbitrarySourceProfile,
                new Direction(0, 0, 1),
                new Position(0,0,0),
                new PolarAzimuthalAngles(),
                0)
            {
                Rng = rng
            };

            // verify 10 photons originate from 2nd pixel i.e. x in [-1.5,-0.5], y in [-0.5,0.5]
            // and normal
            for (var i = 0; i < 10; i++)
            {
                var photon = ps.GetNextPhoton(tissue);
                Assert.IsTrue(photon.DP.Position.X >= -1.5 && photon.DP.Position.X <= -0.5);
                Assert.IsTrue(photon.DP.Position.Y > -0.5 && photon.DP.Position.Y <= 0.5);
                Assert.Less(Math.Abs(photon.DP.Position.Z), 1e-6);
                Assert.Less(Math.Abs(photon.DP.Direction.Ux), 1e-6);
                Assert.Less(Math.Abs(photon.DP.Direction.Uy), 1e-6);
                Assert.Less(Math.Abs(photon.DP.Direction.Uz - 1), 1e-6);

            }

        }
 

    }
}
