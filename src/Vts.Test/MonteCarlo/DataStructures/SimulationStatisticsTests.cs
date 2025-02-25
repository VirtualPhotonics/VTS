using NUnit.Framework;
using System.Collections.Generic;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo
{
    /// <summary>
    /// Test that validates statistics results are valid *with* Russian
    /// Roulette turned on.  Other test of statistics without RR in
    /// AnalogOneLayerDetectorsTests, StatisticsExtensionTests
    /// </summary>
    [TestFixture]
    public class SimulationStatisticsTests
    {
        private SimulationStatistics _simulationStatistics;
        private const long N = 100; 

        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        private readonly List<string> _listOfTestGeneratedFiles = new List<string>()
        {
            "file.txt", // file that captures screen output of MC simulation
            "statistics.txt"
        }; 
        private readonly List<string> _listOfTestGeneratedFolders = new List<string>()
        {
            "Output"
        };

        [OneTimeTearDown]
        public void Clear_folders_and_files()
        {
            foreach (var file in _listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
            foreach (var folder in _listOfTestGeneratedFolders)
            {
                FileIO.DeleteDirectory(folder);
            }
        }

        [OneTimeSetUp]
        public void Execute_Monte_Carlo()
        {
            // delete previously generated files
            Clear_folders_and_files();

            // instantiate common classes
            var simulationOptions = new SimulationOptions(
                0,
                RandomNumberGeneratorType.MersenneTwister,
                AbsorptionWeightingType.Discrete,
                PhaseFunctionType.HenyeyGreenstein,
                new List<DatabaseType>(), // databases to be written
                true, // track statistics
                0.001, // turn on RR with 10x usual threshold
                0);
            var source = new DirectionalPointSourceInput(
                new Position(0.0, 0.0, 0.0),
                new Direction(0.0, 0.0, 1.0),
                0); // start outside tissue
            var detectors = new List<IDetectorInput>
            {
                new RSpecularDetectorInput(),
                new RDiffuseDetectorInput(),
                new ATotalDetectorInput(),
                new TDiffuseDetectorInput()
            };
            var input = new SimulationInput(
                N,
                "Output",
                simulationOptions,
                source,
                new MultiLayerTissueInput(
                    new ITissueRegion[]
                    {
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 5.0),
                            // make mua large to turn on RR faster
                            new OpticalProperties(1.0, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(5.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                detectors);
            new MonteCarloSimulation(input).Run();
            _simulationStatistics = SimulationStatistics.FromFile(input.OutputName + "/statistics.txt");
        }
        /// <summary>
        /// Validate statistics when Russian Roulette is turned out.
        /// For N=1000, photon 835 has StateFlag set with KilledByRussianRoulette,
        /// then PseudoReflectedTissueBoundary, because RR occurs at start of track right
        /// before reflection.  Correctly, the photon does not tally to reflectance.
        /// </summary>
        [Test]
        public void Validate_Russian_Roulette_statistics()
        {
                Assert.That(_simulationStatistics.NumberOfPhotonsSpecularReflected == 3, Is.True);
                Assert.That(_simulationStatistics.NumberOfPhotonsOutTopOfTissue == 30, Is.True);
                Assert.That(_simulationStatistics.NumberOfPhotonsOutBottomOfTissue == 1, Is.True);
                Assert.That(_simulationStatistics.NumberOfPhotonsAbsorbed == 0, Is.True);
                Assert.That(_simulationStatistics.NumberOfPhotonsKilledByRussianRoulette == 66, Is.True);
                Assert.IsTrue(_simulationStatistics.NumberOfPhotonsSpecularReflected +
                    _simulationStatistics.NumberOfPhotonsOutTopOfTissue +
                    _simulationStatistics.NumberOfPhotonsOutBottomOfTissue +
                    _simulationStatistics.NumberOfPhotonsKilledByRussianRoulette == N);
        }
    }
}
