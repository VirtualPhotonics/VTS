using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Detectors
{
    /// <summary>
    /// These tests verify that the specification of a detector fiber processes the exiting photon correctly.
    /// </summary>
    [TestFixture]
    public class DetectorFiberTests
    {
        private SimulationOutput _outputOpen, _outputNA, _outputNANOfAir;
        private SimulationOptions _simulationOptions;
        private ISourceInput _source;
        private ITissueInput _tissue;
        private IList<IDetectorInput> _detectorOpen, _detectorNA, _detectorNANofAir;
        private double _detectorRadius = 1; // debug set to 10

        /// <summary>
        /// Setup input to the MC for a homogeneous one layer tissue with fiber cylinder and specify fiber detector
        /// </summary>
        [OneTimeSetUp]
        public void execute_Monte_Carlo()
        {
            // instantiate common classes
            _simulationOptions = new SimulationOptions(
                0,
                RandomNumberGeneratorType.MersenneTwister,
                AbsorptionWeightingType.Discrete,
                PhaseFunctionType.HenyeyGreenstein,
                new List<DatabaseType>(), 
                false, // track statistics
                0.0, // RR threshold -> 0 = no RR performed
                0);
            _source = new DirectionalPointSourceInput(
                     new Position(0.0, 0.0, 0.0),
                     new Direction(0.0, 0.0, 1.0),
                     1); 
           
            _tissue = new MultiLayerTissueInput(
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.01, 1, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                }
            );

            _detectorOpen = new List<IDetectorInput>
            {
                new SurfaceFiberDetectorInput()
                {
                    Center = new Position(0, 0, 0), 
                    Radius = _detectorRadius, 
                    TallySecondMoment = true,
                    N = 1.4,  
                    NA = double.PositiveInfinity,
                    FinalTissueRegionIndex = 1
                },
                new ROfRhoDetectorInput() // 1mm wide ring to match fiber detector and 2 because beyond goes into 2nd
                {
                    Rho = new DoubleRange(0.0, 2.0, 3),
                    FinalTissueRegionIndex = 1,
                    NA = double.PositiveInfinity,
                    TallySecondMoment =true
                },
            };
            _detectorNA = new List<IDetectorInput>
            {
                new SurfaceFiberDetectorInput()
                {
                    Center = new Position(0, 0, 0), 
                    Radius = _detectorRadius, 
                    TallySecondMoment = true,
                    N = 1.4,
                    FinalTissueRegionIndex = 1,
                    NA = 0.22
                },
                new ROfRhoDetectorInput() // 1mm wide ring to match fiber detector
                {
                    Rho = new DoubleRange(0.0, 2.0, 3),
                    FinalTissueRegionIndex = 1,
                    NA = 0.22,
                    TallySecondMoment = true
                },
            };
            var _inputOpen = new SimulationInput(
                100,
                "",
                _simulationOptions,
                _source,
                _tissue,
                _detectorOpen);
            _outputOpen = new MonteCarloSimulation(_inputOpen).Run();

            var _inputNA = new SimulationInput(
                100,
                "",
                _simulationOptions,
                _source,
                _tissue,
                _detectorNA);
            _outputNA = new MonteCarloSimulation(_inputNA).Run();
        }

        /// <summary>
        /// Test to validate fiber at tissue surface fully open. Validation values based on prior test.
        /// [mean-3sig, mean+3sig]=[0.017, 0.057]
        /// </summary>
        [Test]
        public void validate_fully_open_surface_fiber_detector_produces_correct_results()
        {
            // R_r[0] should be different than SurFib because SurFib has an N different than air
            Assert.Less(Math.Abs(_outputOpen.R_r[0] - 0.039926), 0.000001);
            Assert.Less(Math.Abs(_outputOpen.SurFib - 0.037243), 0.000001);
            Assert.Less(Math.Abs(_outputOpen.SurFib2 - 0.005786), 0.000001);
            var sd = Math.Sqrt((_outputOpen.SurFib2 -
                                _outputOpen.SurFib * _outputOpen.SurFib) / 100);
            var threeSigmaPos = _outputOpen.SurFib + 3 * sd;
            var threeSigmaNeg = _outputOpen.SurFib - 3 * sd;
            Assert.AreEqual(_outputOpen.SurFib_TallyCount, 24);
        }
        /// <summary>
        /// Test to validate fiber at tissue surface fully open. Validation values based on prior test.
        /// Theory [Bargo et al., AO 42(16) 2003] states (R_NA/R_Open)=(NA/ntiss)^2
        /// (0.22/1.4)^2=0.0246
        /// taking means of both (R_NA/R_Open)=0.035 -> not so good
        //// [mean-3sig, mean+3sig]=[0.0, 0.0053] -> agreement
        /// but if take 3sigma range may agree
        /// </summary>
        [Test]
        public void validate_NA_surface_fiber_detector_produces_correct_results()
        {
            Assert.Less(Math.Abs(_outputNA.R_r[0] - 0.002643), 0.000001);
            Assert.Less(Math.Abs(_outputNA.SurFib - 0.001321), 0.000001);
            Assert.Less(Math.Abs(_outputNA.SurFib2 - 0.000174), 0.000001);
            var sd = Math.Sqrt((_outputNA.SurFib2 -
                                _outputNA.SurFib * _outputNA.SurFib) / 100);
            var threeSigmaPos = _outputNA.SurFib + 3 * sd;
            var threeSigmaNeg = _outputNA.SurFib - 3 * sd;
            Assert.AreEqual(_outputNA.SurFib_TallyCount, 1);
        }
    }
}
