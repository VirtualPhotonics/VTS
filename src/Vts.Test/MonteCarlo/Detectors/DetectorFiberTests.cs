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
        private SimulationOutput _outputWithOpenSurfaceFiber, _outputWithNA1p0SurfaceFiber;
        private SimulationOptions _simulationOptions;
        private ISourceInput _source;
        private ITissueInput _tissueWithSurfaceFiber;
        private IList<IDetectorInput> _detectorSurfaceOpen, _detectorSurfaceNA1p0;
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
           
            _tissueWithSurfaceFiber = new MultiLayerTissueInput(
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

            _detectorSurfaceOpen = new List<IDetectorInput>
            {
                new SurfaceFiberDetectorInput()
                {
                    Center = new Position(0, 0, 0), 
                    Radius = _detectorRadius, 
                    TallySecondMoment = true,
                    N = 1.4,  
                    FinalTissueRegionIndex = 1
                },
                new ROfRhoDetectorInput()
                {
                    Rho = new DoubleRange(0.0, 10.0, 11), FinalTissueRegionIndex= 0, TallySecondMoment=true
                },
            };
            _detectorSurfaceNA1p0 = new List<IDetectorInput>
            {
                new SurfaceFiberDetectorInput()
                {
                    Center = new Position(0, 0, 0), 
                    Radius = _detectorRadius, 
                    TallySecondMoment = true,
                    N = 1.4,
                    FinalTissueRegionIndex = 1,
                    NA = 1.0
                },
                new ROfRhoDetectorInput() // 1mm wide rings
                {
                    Rho = new DoubleRange(0.0, 10.0, 11), FinalTissueRegionIndex= 0, TallySecondMoment = true
                },
            };
            var _inputWithOpenSurfaceFiber = new SimulationInput(
                100,
                "",
                _simulationOptions,
                _source,
                _tissueWithSurfaceFiber,
                _detectorSurfaceOpen);
            _outputWithOpenSurfaceFiber = new MonteCarloSimulation(_inputWithOpenSurfaceFiber).Run();

            var _inputWithNA1p0SurfaceFiber = new SimulationInput(
                100,
                "",
                _simulationOptions,
                _source,
                _tissueWithSurfaceFiber,
                _detectorSurfaceNA1p0);
            _outputWithNA1p0SurfaceFiber = new MonteCarloSimulation(_inputWithNA1p0SurfaceFiber).Run();
        }

        /// <summary>
        /// Test to validate fiber at tissue surface fully open. Validation values based on prior test.
        /// [mean-3sig, mean+3sig]=[0.0045, 0.0354]
        /// </summary>
        [Test]
        public void validate_fully_open_surface_fiber_detector_produces_correct_results()
        {
            Assert.Less(Math.Abs(_outputWithOpenSurfaceFiber.R_r[0] - 0.039926), 0.000001);
            Assert.Less(Math.Abs(_outputWithOpenSurfaceFiber.SurFib - 0.019963), 0.000001);
            Assert.Less(Math.Abs(_outputWithOpenSurfaceFiber.SurFib2 - 0.003071), 0.000001);
            var sd = Math.Sqrt((_outputWithOpenSurfaceFiber.SurFib2 -
                                _outputWithOpenSurfaceFiber.SurFib * _outputWithOpenSurfaceFiber.SurFib) / 100);
            var threeSigmaPos = _outputWithOpenSurfaceFiber.SurFib + 3 * sd;
            var threeSigmaNeg = _outputWithOpenSurfaceFiber.SurFib - 3 * sd;
            Assert.AreEqual(_outputWithOpenSurfaceFiber.SurFib_TallyCount, 13);
        }
        /// <summary>
        /// Test to validate fiber at tissue surface fully open. Validation values based on prior test.
        /// Theory [Bargo et al., AO 42(16) 2003] states (R_NA/R_Open)=(NA/ntiss)^2
        /// (1.0/1.4)^2=0.510
        /// taking means of both (R_NA/R_Open)=0.343 -> not so good but if take 3sigma range may agree
        /// </summary>
        [Test]
        public void validate_NA1p0_surface_fiber_detector_produces_correct_results()
        {
            Assert.Less(Math.Abs(_outputWithNA1p0SurfaceFiber.R_r[0] - 0.039926), 0.000001);
            Assert.Less(Math.Abs(_outputWithNA1p0SurfaceFiber.SurFib - 0.013704), 0.000001);
            Assert.Less(Math.Abs(_outputWithNA1p0SurfaceFiber.SurFib2 - 0.002091), 0.000001);
            var sd = Math.Sqrt((_outputWithNA1p0SurfaceFiber.SurFib2 -
                                _outputWithNA1p0SurfaceFiber.SurFib * _outputWithNA1p0SurfaceFiber.SurFib) / 100);
            var threeSigmaPos = _outputWithNA1p0SurfaceFiber.SurFib + 3 * sd;
            var threeSigmaNeg = _outputWithNA1p0SurfaceFiber.SurFib - 3 * sd;
            Assert.AreEqual(_outputWithNA1p0SurfaceFiber.SurFib_TallyCount, 9);
        }
    }
}
