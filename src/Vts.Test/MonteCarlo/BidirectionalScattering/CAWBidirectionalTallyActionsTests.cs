using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.BidirectionalScattering
{
    /// <summary>
    /// These tests execute an CAW MC bidirectional simulation with 1e4 photons and verify
    /// that the tally results match the analytic solution within variance
    /// mersenne twister STANDARD_TEST.
    /// These solutions assume index matched slab.
    /// </summary>
    [TestFixture]
    public class CAWBidirectionalTallyActionsTests
    {
        SimulationOutput _output;
        SimulationInput _input;
        readonly double _slabThickness = 10;
        readonly double _mua = 0.01;
        readonly double _musp = 0.198;  // mus = 0.99
        readonly double _g = 0.8;

        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        readonly List<string> listOfTestGeneratedFolders = new List<string>()
        {
            "results"
        };

        readonly List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "file.txt"  // file that captures the screen output of MC simulation
        };

        [OneTimeTearDown]
        public void clear_folders_and_files()
        {
            foreach (var folder in listOfTestGeneratedFolders)
            {
                FileIO.DeleteDirectory(folder);
            }
            foreach (var file in listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
        }
        /// <summary>
        /// Setup input to the MC, SimulationInput, and execute MC
        /// </summary>
        [OneTimeSetUp]
        public void execute_Monte_Carlo()
        {
            // delete any previously generated files and folders 
            clear_folders_and_files();

            var tissue = new MultiLayerTissueInput(
                new ITissueRegion[]
                {
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 0.0, 1.0),
                            "BidirectionalKey1"),
                        //new LayerTissueRegion( // debug layer
                        //    new DoubleRange(0.0, _topLayerThickness),
                        //    new OpticalProperties(_mua, _musp, _g, 1.0)), // index matched slab                        
                        new LayerTissueRegion(
                            new DoubleRange(0, _slabThickness),
                            new OpticalProperties(_mua, _musp, _g, 1.0),
                            "BidirectionalKey2"), // index matched slab
                        new LayerTissueRegion(
                            new DoubleRange(_slabThickness, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 0.0, 1.0),
                            "BidirectionalKey3")
                }
            );                            
            tissue.RegionPhaseFunctionInputs.Add("BidirectionalKey1", new BidirectionalPhaseFunctionInput());
            tissue.RegionPhaseFunctionInputs.Add("BidirectionalKey2", new BidirectionalPhaseFunctionInput());
            tissue.RegionPhaseFunctionInputs.Add("BidirectionalKey3", new BidirectionalPhaseFunctionInput());

            _input = new SimulationInput(
                10000, // 1e4 needed to get enough photons to Td 
                "results",
                new SimulationOptions(
                    0, 
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Continuous, 
                    new List<DatabaseType>() { }, // databases to be written
                    true, // track statistics
                    0.0, // RR threshold -> 0 = no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0                   
                ),
                tissue,
                new List<IDetectorInput>() 
                { 
                    new RDiffuseDetectorInput() { TallySecondMoment = true },
                    new ATotalDetectorInput() { TallySecondMoment = true },
                    new TDiffuseDetectorInput() { TallySecondMoment = true }
                }
            );
            _output = new MonteCarloSimulation(_input).Run();
        }

        // Diffuse Reflectance
        [Test]
        public void validate_bidirectional_CAW_RDiffuse()
        {
            var analyticSolution = BidirectionalAnalyticSolutions.GetBidirectionalRadianceInSlab(
                _slabThickness,
                new OpticalProperties(_mua, _musp, _g, 1.0),
                -1, // direction -1=up
                0); // position at surface
            var sd = ErrorCalculation.StandardDeviation(_output.Input.N, _output.Rd, _output.Rd2);
            Assert.That(Math.Abs(_output.Rd - analyticSolution), Is.LessThan(3 * sd)); 
        }
        // Total Absorption
        [Test]
        public void validate_bidirectional_CAW_ATotal()
        {
            var analyticSolutionRight =
                BidirectionalAnalyticSolutions.GetBidirectionalRadianceIntegratedOverInterval(
                _slabThickness,
                new OpticalProperties(_mua, _musp, _g, 1.0),
                1,
                0,
                _slabThickness);
            var analyticSolutionLeft =
                BidirectionalAnalyticSolutions.GetBidirectionalRadianceIntegratedOverInterval(
                _slabThickness,
                new OpticalProperties(_mua, _musp, _g, 1.0),
                -1,
                0,
                _slabThickness);
            // take sum because absorbed energy independent of direction
            var analyticSolution = (analyticSolutionRight + analyticSolutionLeft);
            var sd = ErrorCalculation.StandardDeviation(_output.Input.N, _output.Atot, _output.Atot2);
            Assert.That(Math.Abs(_output.Atot - _mua * analyticSolution), Is.LessThan(3 * sd));
        }
        // Diffuse Transmittance
        [Test]
        public void validate_bidirectional_CAW_TDiffuse()
        {
            var analyticSolution = BidirectionalAnalyticSolutions.GetBidirectionalRadianceInSlab(
                _slabThickness,
                new OpticalProperties(_mua, _musp, _g, 1.0),
                1, // direction 1=down
                _slabThickness); // position at slab end
            var sd = ErrorCalculation.StandardDeviation(_output.Input.N, _output.Td, _output.Td2);
            Assert.That(Math.Abs(_output.Td - analyticSolution), Is.LessThan(3 * sd));
        }
        // with no refractive index mismatch, Rd + Atot + Td should equal 1
        [Test]
        public void validate_bidirectional_CAW_detector_sum_equals_one()
        {
            Assert.That(Math.Abs(_output.Rd + _output.Atot + _output.Td - 1.0), Is.LessThan(0.00000000001));
        }
    }
}
