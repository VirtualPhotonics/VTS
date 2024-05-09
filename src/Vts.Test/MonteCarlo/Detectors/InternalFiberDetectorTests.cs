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
    /// These tests verify that the specification of an internal surface fiber detector.
    /// </summary>
    [TestFixture]
    public class InternalSurfaceFiberDetectorTests
    {
        private SimulationOptions _simulationOptions;
        private ISourceInput _sourceOutsideSurface;
        private ITissueInput _tissueWithCylinders, _tissueWithLayers, _tissueWithAirLayer;
        private IList<IDetectorInput> _detectorOuterCylinderDotNormalPositiveOpen, 
            _detectorOuterCylinderDotNormalPositiveNa,
            _detectorInnerCylinderDotNormalNegativeOpen,
            _detectorInnerCylinderDotNormalNegativeNa,
            _detectorAtopSecondLayerOpen;
        private SimulationOutput _outputOuterCylinderDotNormalPositiveOpen, 
            _outputOuterCylinderDotNormalPositiveNa,
            _outputInnerCylinderDotNormalNegativeOpen,
            _outputInnerCylinderDotNormalNegativeNa,
            _outputAtopSecondTissueLayer, 
            _outputAtopSecondAirLayer;
        private const double DetectorRadius = 1; // make big so catch more

        /// <summary>
        /// Setup input to the MC for a homogeneous one layer tissue with 
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
            _sourceOutsideSurface = new DirectionalPointSourceInput(
                     new Position(0.0, 0.0, 0.0),
                     new Direction(0.0, 0.0, 1.0),
                     1);
            _tissueWithCylinders =
                new MultiConcentricInfiniteCylinderTissueInput(
                    new ITissueRegion[]
                    {
                        new InfiniteCylinderTissueRegion(
                            new Position(0, 0, 2),
                            1.0,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4)),
                        new InfiniteCylinderTissueRegion(
                            new Position(0, 0, 2),
                            0.5,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4))
                    },
                    new ITissueRegion[]
                    {
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                );
            // need to spell these out in separate simulations because SimulationOutput
            // has no way to distinguish 
            _detectorOuterCylinderDotNormalPositiveOpen = new List<IDetectorInput>
            {
                new InternalSurfaceFiberDetectorInput
                {
                    Center = new Position(0, 0, 1),
                    Radius = DetectorRadius,
                    TallySecondMoment = true,
                    N = 1.4,
                    NA = 1.4,
                    FinalTissueRegionIndex = 3,
                    InDirectionOfFiberAxis = new Direction(0, 0, -1)
                }
            }; 
            _detectorOuterCylinderDotNormalPositiveNa = new List<IDetectorInput>
            {
                new InternalSurfaceFiberDetectorInput
                {
                    Center = new Position(0, 0, 1),
                    Radius = DetectorRadius,
                    TallySecondMoment = true,
                    N = 1.4,
                    NA = 1.0, // tried many values here to obtain different tally than open, either all or nothing I found
                    FinalTissueRegionIndex = 3,
                    InDirectionOfFiberAxis = new Direction(0, 0, -1)
                }
            };
            _detectorInnerCylinderDotNormalNegativeOpen = new List<IDetectorInput>
                {
                new InternalSurfaceFiberDetectorInput
                {
                    Center = new Position(0, 0, 1.5),
                    Radius = DetectorRadius,
                    TallySecondMoment = true,
                    N = 1.4,
                    NA = 1.4,
                    FinalTissueRegionIndex = 4,
                    InDirectionOfFiberAxis = new Direction(0,0, 1),
                }
            };
            _detectorInnerCylinderDotNormalNegativeNa = new List<IDetectorInput>
            {
                new InternalSurfaceFiberDetectorInput
                {
                    Center = new Position(0, 0, 1.5),
                    Radius = DetectorRadius,
                    TallySecondMoment = true,
                    N = 1.4,
                    NA = 0.8, // make smaller but not too small so some still tally but different than open
                    FinalTissueRegionIndex = 4,
                    InDirectionOfFiberAxis = new Direction(0,0, 1),
                }
            };

            var input = new SimulationInput(
                100,
                "",
                _simulationOptions,
                _sourceOutsideSurface,
                _tissueWithCylinders,
                _detectorOuterCylinderDotNormalPositiveOpen);
            _outputOuterCylinderDotNormalPositiveOpen = new MonteCarloSimulation(input).Run();

            input = new SimulationInput(
                100,
                "",
                _simulationOptions,
                _sourceOutsideSurface,
                _tissueWithCylinders,
                _detectorOuterCylinderDotNormalPositiveNa);
            _outputOuterCylinderDotNormalPositiveNa = new MonteCarloSimulation(input).Run();

            input = new SimulationInput(
                100,
                "",
                _simulationOptions,
                _sourceOutsideSurface,
                _tissueWithCylinders,
                _detectorInnerCylinderDotNormalNegativeOpen);
            _outputInnerCylinderDotNormalNegativeOpen = new MonteCarloSimulation(input).Run();

            input = new SimulationInput(
                100,
                "",
                _simulationOptions,
                _sourceOutsideSurface,
                _tissueWithCylinders,
                _detectorInnerCylinderDotNormalNegativeNa);
            _outputInnerCylinderDotNormalNegativeNa = new MonteCarloSimulation(input).Run();

            // Specify layers with internal fiber atop second layer
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
                    new ITissueRegion[]
                    {
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
                    }
                );
            input = new SimulationInput(
                100,
                "",
                _simulationOptions,
                _sourceOutsideSurface,
                _tissueWithLayers,
                _detectorAtopSecondLayerOpen); // this is below bottom of 2nd air layer
            _outputAtopSecondTissueLayer = new MonteCarloSimulation(input).Run();

            _tissueWithAirLayer =
                new MultiLayerTissueInput(
                    new ITissueRegion[]
                    {
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
                    }
                );
            input = new SimulationInput(
                100,
                "",
                _simulationOptions,
                _sourceOutsideSurface,
                _tissueWithAirLayer,
                _detectorAtopSecondLayerOpen); // this is below bottom of 2nd air layer
            _outputAtopSecondAirLayer = new MonteCarloSimulation(input).Run();
        }

        /// <summary>
        /// Test to validate fiber at outer cylinder surface fully open with dot normal positive. 
        /// Validation values based on prior test.  
        /// </summary>
        [Test]
        public void Validate_fully_open_fiber_on_outer_cylinder_produces_correct_results()
        {
            Assert.Less(Math.Abs(_outputOuterCylinderDotNormalPositiveOpen.IntSurFib - 0.047083), 0.000001);
            Assert.Less(Math.Abs(_outputOuterCylinderDotNormalPositiveOpen.IntSurFib2 - 0.014778), 0.000001);
            Assert.AreEqual(15, _outputOuterCylinderDotNormalPositiveOpen.IntSurFib_TallyCount);
        }

        /// <summary>
        /// Test to validate fiber at outer cylinder surface smaller NA with dot normal positive. 
        /// Validation values based on prior test.  Tried NA=1.1 (15 photons), NA=1.0 (none)
        /// </summary>
        [Test]
        public void Validate_NA_fiber_on_outer_cylinder_produces_correct_results()
        {
            // these results are smaller than open NA results for same fiber location as they should be
            Assert.Less(Math.Abs(_outputOuterCylinderDotNormalPositiveNa.IntSurFib - 0.0), 0.000001);
            Assert.Less(Math.Abs(_outputOuterCylinderDotNormalPositiveNa.IntSurFib2 - 0.0), 0.000001);
            Assert.AreEqual(0, _outputOuterCylinderDotNormalPositiveNa.IntSurFib_TallyCount);
        }

        /// <summary>
        /// Test to validate fiber at inner cylinder surface fully open with dot normal negative
        /// Validation values based on prior test.
        /// </summary>
        [Test]
        public void Validate_fully_open_fiber_on_inner_cylinder_produces_correct_results()
        {
            Assert.Less(Math.Abs(_outputInnerCylinderDotNormalNegativeOpen.IntSurFib - 0.009330), 0.000001);
            Assert.Less(Math.Abs(_outputInnerCylinderDotNormalNegativeOpen.IntSurFib2 - 0.002902), 0.000001);
            Assert.AreEqual(3, _outputInnerCylinderDotNormalNegativeOpen.IntSurFib_TallyCount);
        }

        /// <summary>
        /// Test to validate fiber at inner cylinder surface smaller NA with dot normal negative. 
        /// Validation values based on prior test.
        /// </summary>
        [Test]
        public void Validate_NA_fiber_on_inner_cylinder_produces_correct_results()
        {
            // these results are smaller than open NA results for same fiber location as they should be
            Assert.Less(Math.Abs(_outputInnerCylinderDotNormalNegativeNa.IntSurFib - 0.006278), 0.000001);
            Assert.Less(Math.Abs(_outputInnerCylinderDotNormalNegativeNa.IntSurFib2 - 0.001970), 0.000001);
            Assert.AreEqual(2, _outputInnerCylinderDotNormalNegativeNa.IntSurFib_TallyCount);
        }

        /// <summary>
        /// Test to validate fiber at 2nd tissue layer surface fully open with dot normal positive. 
        /// Validation values based on prior test.
        /// </summary>
        [Test]
        public void Validate_fully_open_fiber_atop_second_layer_below_tissue_produces_correct_results()
        {
            Assert.Less(Math.Abs(_outputAtopSecondTissueLayer.IntSurFib - 0.024520), 0.000001);
            Assert.Less(Math.Abs(_outputAtopSecondTissueLayer.IntSurFib2 - 0.007525), 0.000001);
            Assert.AreEqual(8, _outputAtopSecondTissueLayer.IntSurFib_TallyCount);
        }

        /// <summary>
        /// Test to validate fiber at 2nd air layer surface fully open with dot normal positive. 
        /// Validation values based on prior test.
        /// </summary>
        [Test]
        public void Validate_fully_open_fiber_atop_second_layer_below_air_produces_correct_results()
        {
            Assert.Less(Math.Abs(_outputAtopSecondAirLayer.IntSurFib - 0.051616), 0.000001);
            Assert.Less(Math.Abs(_outputAtopSecondAirLayer.IntSurFib2 - 0.015710), 0.000001);
            Assert.AreEqual(17, _outputAtopSecondAirLayer.IntSurFib_TallyCount);
        }
    }
}
