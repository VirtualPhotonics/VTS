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
    /// Unit tests for Surface Emitting Sources: DirectionalArbitrarySources
    /// Note: the test image in Resources, "circle.png" was created by using
    /// Inkscape and saving to a png file.
    /// </summary>
    [TestFixture]
    public class DirectionalArbitrarySourceTests
    {
        DirectionalArbitrarySourceInput _sourceInput;

        [OneTimeSetUp]
        public void Setup()
        {
            // read in image file locally
            const string folder = "";
            var name = Assembly.GetExecutingAssembly().FullName;
            if (name == null) return; 
            var assemblyName = new AssemblyName(name).Name;
            FileIO.CopyFileFromEmbeddedResources(
                assemblyName + ".Resources.circle.png", 
                Path.Combine(folder, "circle.png"), 
                name);
            // set up infile
            _sourceInput = new DirectionalArbitrarySourceInput(
                "",
                "circle.png",
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
            var source = new DirectionalArbitrarySourceInput(
                _sourceInput.InputFolder,
                _sourceInput.ImageName,
                _sourceInput.NumberOfPixelsX,
                _sourceInput.NumberOfPixelsY,
                _sourceInput.PixelWidthX,
                _sourceInput.PixelHeightY,
                SourceProfileType.Arbitrary);
            Assert.AreEqual(113, source.NumberOfPixelsX);
            Assert.AreEqual(102, source.NumberOfPixelsY);
            Assert.AreEqual(0.1, source.PixelWidthX);
            Assert.AreEqual(0.1, source.PixelHeightY);
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
            Assert.IsTrue(output.T_xy[14, 14] > 0.0); // center of image
            Assert.IsTrue(Math.Abs(output.T_xy[0, 0] - 0.0) < 1e-6); // corner of image
        }

        /// <summary>
        /// Instantiate ArbitrarySourceProfile and validate GetBinaryPixelMap,
        /// then use this object in general constructor of DirectionalArbitrarySource
        /// and validate initiated new Photon
        /// </summary>
        [Test]
        public void Validate_binary_map_and_general_constructor_for_directional_arbitrary_source_test()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            // make a intensity varied source
            var image = new double[] { 1, 2, 2, 0 }; // representing a 2x2 pixel image
            var arbitrarySourceProfile = new ArbitrarySourceProfile(
                image,
                2,
                2,
                1,
                1,
                new Vts.Common.Position(0, 0, 0));
            var binaryMap = arbitrarySourceProfile.GetBinaryPixelMap();
            Assert.IsTrue(binaryMap[0] == 1);
            Assert.IsTrue(binaryMap[1] == 1);
            Assert.IsTrue(binaryMap[2] == 1);
            Assert.IsTrue(binaryMap[3] == 0);

            var ps = new DirectionalArbitrarySource(
                0.0, // normal
                2.0,
                2.0,
                arbitrarySourceProfile,
                new Direction(0, 0, 1),
                new Position(0,0,0),
                new PolarAzimuthalAngles(),
                0)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            // normal launch
            Assert.Less(Math.Abs(photon.DP.Direction.Ux), 1e-6);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy), 1e-6);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - 1), 1e-6);

            Assert.Less(Math.Abs(photon.DP.Position.X + 0.5), 1e-6);
            Assert.Less(Math.Abs(photon.DP.Position.Y - 0.5), 1e-6);
            Assert.Less(Math.Abs(photon.DP.Position.Z), 1e-6);
        }
 

    }
}
