using NUnit.Framework;
using System;
using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Detectors
{
    /// <summary>
    /// These tests verify that the specification of an internal surface fiber detector.
    /// </summary>
    [TestFixture]
    public class FiberInternalSurfaceDetectorsTests
    {
        private SimulationOptions _simulationOptions;
        private ISourceInput _sourceAtopFirstLayer, _sourceAtopOuterCylinder;
        private ITissueInput _tissueWithLayers, _tissueWithAirLayer, _tissueWithCylinders;
        private IList<IDetectorInput> _detectorAtopSecondLayerOpen, _detectorsCylinderSystem;
        private SimulationInput _inputAtopSecondTissueLayer, _inputAtopSecondAirLayer, _inputCylinderSystem;
        private SimulationOutput _outputAtopSecondTissueLayer, _outputAtopSecondAirLayer, _outputCylinderSystem;
        private const double DetectorAtopCylinderRadius = 1.0; // make bigger so catch more

        /// <summary>
        /// Setup input to the MC for:
        /// Case 1) two layer tissue (air-layer-layer-air) with source and detector atop 2nd layer,
        /// Case 2) two layer tissue (air-air-layer-air) with source and detector atop 2nd layer, and
        /// Case 3) homogeneous one layer tissue with 
        /// multi-concentric infinite cylinder and put fibers on outside outermost cylinder surface
        /// (dot product with surface normal is positive) and inside innermost cylinder surface
        /// (dot product with surface normal is negative) and make sure detection is correct
        /// </summary>
        [OneTimeSetUp]
        public void Execute_Monte_Carlo()
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
            _sourceAtopFirstLayer = new DirectionalPointSourceInput(
                new Position(0.0, 0.0, 1.0), // atop 2nd "tissue" layer
                new Direction(0.0, 0.0, 1.0),
                1); // start above 2nd "tissue" layer

            // Specify air-tissue-tissue-air system with internal fiber atop second layer
            _detectorAtopSecondLayerOpen = new List<IDetectorInput>
            {
                new InternalSurfaceFiberDetectorInput
                {
                    Center = new Position(0, 0, 1), // atop 2nd "tissue" layer
                    Radius = 1.0,
                    TallySecondMoment = true,
                    N = 1.4,
                    NA = 1.4,
                    FinalTissueRegionIndex = 1, // atop 2nd "tissue" layer
                    InDirectionOfFiberAxis = new Direction(0, 0, -1)
                }
            };
            _tissueWithLayers =
                new MultiLayerTissueInput(
                    [
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion( // tissue in top layer
                            new DoubleRange(0.0, 1.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(1.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    ]
                );
            _inputAtopSecondTissueLayer = new SimulationInput(
                100,
                "",
                _simulationOptions,
                _sourceAtopFirstLayer,
                _tissueWithLayers,
                _detectorAtopSecondLayerOpen); // this is below bottom of 2nd air layer
            _outputAtopSecondTissueLayer = new MonteCarloSimulation(_inputAtopSecondTissueLayer).Run();

            // Specify air-air-tissue-air system with internal fiber atop second layer
            _tissueWithAirLayer =
                new MultiLayerTissueInput(
                    [
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion( // put "air" as first layer nonsensical but unit test
                            new DoubleRange(0.0, 1.0),
                            new OpticalProperties(1E-100, 1E-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(1.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    ]
                );
            _inputAtopSecondAirLayer = new SimulationInput(
                100,
                "",
                _simulationOptions,
                _sourceAtopFirstLayer,
                _tissueWithAirLayer,
                _detectorAtopSecondLayerOpen); // this is below bottom of 2nd air layer
            _outputAtopSecondAirLayer = new MonteCarloSimulation(_inputAtopSecondAirLayer).Run();

            // Case 3) Concentric cylinders inside single layer of tissue
            // Specify air-tissue(with 2 concentric cylinders)-air system with internal fiber atop
            // outer and inner cylinder
            _sourceAtopOuterCylinder = new DirectionalPointSourceInput(
                new Position(0.0, 0.0, 1.0), // atop outer cylinder
                new Direction(0.0, 0.0, 1.0),
                1); // start in surrounding tissue
            _tissueWithCylinders =
                new MultiConcentricInfiniteCylinderTissueInput(
                    [
                        new InfiniteCylinderTissueRegion(
                            new Position(0, 0, 10),
                            9.0,
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new InfiniteCylinderTissueRegion(
                            new Position(0, 0, 10),
                            8.0,
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4))
                    ],
                    [
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    ]
                ); // using SimulationOutput AllInternalFiberDetectorMeans definition
            _detectorsCylinderSystem = new List<IDetectorInput>
            {
                new InternalSurfaceFiberDetectorInput // outer cylinder dot normal positive open
                {
                    Center = new Position(0, 0, 1.0),
                    Radius = DetectorAtopCylinderRadius,
                    TallySecondMoment = true,
                    N = 1.4,
                    NA = 1.4,
                    FinalTissueRegionIndex = 3, // has to be index of cylinder it atop
                    InDirectionOfFiberAxis = new Direction(0, 0, -1),
                    Name = "InternalSurfaceFiber1"
                },
                new InternalSurfaceFiberDetectorInput // outer cylinder dot normal positive NA
                {
                    Center = new Position(0, 0, 1.0),
                    Radius = DetectorAtopCylinderRadius,
                    TallySecondMoment = true,
                    N = 1.4,
                    NA = 1.0, // tried many values here to obtain different tally than open, either all or nothing I found
                    FinalTissueRegionIndex = 3,
                    InDirectionOfFiberAxis = new Direction(0, 0, -1),
                    Name = "InternalSurfaceFiber2"
                },
                new InternalSurfaceFiberDetectorInput // inner cylinder dot normal negative open
                {
                    Center = new Position(0, 0, 2.0),
                    Radius = DetectorAtopCylinderRadius,
                    TallySecondMoment = true,
                    N = 1.4,
                    NA = 1.4,
                    FinalTissueRegionIndex = 4,
                    InDirectionOfFiberAxis = new Direction(0, 0, 1),
                    Name = "InternalSurfaceFiber3"
                },
                new InternalSurfaceFiberDetectorInput // inner cylinder dot normal negative NA
                {
                    Center = new Position(0, 0, 2.0),
                    Radius = DetectorAtopCylinderRadius,
                    TallySecondMoment = true,
                    N = 1.4,
                    NA = 1.2, // make smaller but not too small so some still tally but different than open
                    FinalTissueRegionIndex = 4,
                    InDirectionOfFiberAxis = new Direction(0, 0, 1),
                    Name = "InternalSurfaceFiber4"
                }
            };
            _inputCylinderSystem = new SimulationInput(
                100,
                "",
                _simulationOptions,
                _sourceAtopOuterCylinder,
                _tissueWithCylinders,
                _detectorsCylinderSystem);
            _outputCylinderSystem = new MonteCarloSimulation(_inputCylinderSystem).Run();
        }

        /// <summary>
        /// Case 1) Test to validate fiber at 2nd tissue layer surface fully open with dot normal positive. 
        /// Validation values based on prior test.
        /// </summary>
        [Test]
        public void Validate_fully_open_fiber_atop_second_layer_below_tissue_produces_correct_results()
        {
            Assert.Less(Math.Abs(_outputAtopSecondTissueLayer.IntSurFib - 0.073386), 0.000001);
            Assert.Less(Math.Abs(_outputAtopSecondTissueLayer.IntSurFib2 - 0.022474), 0.000001);
            Assert.AreEqual(24, _outputAtopSecondTissueLayer.IntSurFib_TallyCount);
        }

        /// <summary>
        /// Case 2) Test to validate fiber at 2nd air layer surface fully open with dot normal positive. 
        /// Validation values based on prior test.
        /// </summary>
        [Test]
        public void Validate_fully_open_fiber_atop_second_layer_below_air_produces_correct_results()
        {
            Assert.Less(Math.Abs(_outputAtopSecondAirLayer.IntSurFib - 0.051616), 0.000001);
            Assert.Less(Math.Abs(_outputAtopSecondAirLayer.IntSurFib2 - 0.015710), 0.000001);
            Assert.AreEqual(17, _outputAtopSecondAirLayer.IntSurFib_TallyCount);
        }

        /// <summary>
        /// Case 3) Test to validate fiber at outer cylinder surface fully open with dot normal positive. 
        /// Validation values based on prior test.  
        /// </summary>
        [Test]
        public void Validate_fully_open_fiber_on_outer_cylinder_produces_correct_results()
        {
            var detector = (InternalSurfaceFiberDetector)_outputCylinderSystem.ResultsDictionary["InternalSurfaceFiber1"];
            Assert.Less(Math.Abs(detector.Mean - 0.090464), 0.000001);
            Assert.Less(Math.Abs(detector.SecondMoment - 0.027442), 0.000001);
            Assert.AreEqual(30, detector.TallyCount);
        }

        /// <summary>
        /// Case 3) Test to validate fiber at outer cylinder surface smaller NA with dot normal positive. 
        /// Validation values based on prior test.  
        /// </summary>
        [Test]
        public void Validate_NA_fiber_on_outer_cylinder_produces_correct_results()
        {
            // these results are smaller than open NA results for same fiber location as they should be
            var detector = (InternalSurfaceFiberDetector)_outputCylinderSystem.ResultsDictionary["InternalSurfaceFiber2"];
            Assert.Less(Math.Abs(detector.Mean - 0.042396), 0.000001);
            Assert.Less(Math.Abs(detector.SecondMoment - 0.012859), 0.000001);
            Assert.AreEqual(14, detector.TallyCount);
        }

        /// <summary>
        /// Case 3) Test to validate fiber at inner cylinder surface fully open with dot normal negative
        /// Validation values based on prior test.
        /// </summary>
        [Test]
        public void Validate_fully_open_fiber_on_inner_cylinder_produces_correct_results()
        {
            var detector = (InternalSurfaceFiberDetector)_outputCylinderSystem.ResultsDictionary["InternalSurfaceFiber3"];
            Assert.Less(Math.Abs(detector.Mean - 0.020568), 0.000001);
            Assert.Less(Math.Abs(detector.SecondMoment - 0.006091), 0.000001);
            Assert.AreEqual(7, detector.TallyCount);
        }

        /// <summary>
        /// Case 3) Test to validate fiber at inner cylinder surface smaller NA with dot normal negative. 
        /// Validation values based on prior test.
        /// </summary>
        [Test]
        public void Validate_NA_fiber_on_inner_cylinder_produces_correct_results()
        {
            var detector = (InternalSurfaceFiberDetector)_outputCylinderSystem.ResultsDictionary["InternalSurfaceFiber4"];
            // these results are smaller than open NA results for same fiber location as they should be
            Assert.Less(Math.Abs(detector.Mean - 0.006058), 0.000001);
            Assert.Less(Math.Abs(detector.SecondMoment - 0.001836), 0.000001);
            Assert.AreEqual(2, detector.TallyCount);
        }
    }
}
