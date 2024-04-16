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
        private SimulationOutput _outputOuterOpen, _outputInnerOpen, _outputOuterNA, _outputInnerNA;
        private SimulationOptions _simulationOptions;
        private ISourceInput _source;
        private ITissueInput _tissue;
        private IList<IDetectorInput> _detectorOuterOpen, _detectorInnerOpen, _detectorOuterNA, _detectorInnerNA;
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
            _source = new DirectionalPointSourceInput(
                     new Position(0.0, 0.0, 0.0),
                     new Direction(0.0, 0.0, 1.0),
                     1);
            _tissue =
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
            _detectorOuterOpen = new List<IDetectorInput>
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
            _detectorInnerOpen = new List<IDetectorInput>
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
            _detectorOuterNA = new List<IDetectorInput>
            {
                new InternalSurfaceFiberDetectorInput
                {
                    Center = new Position(0, 0, 1),
                    Radius = DetectorRadius,
                    TallySecondMoment = true,
                    N = 1.4,
                    NA = 1.0, // make smaller but not too small so some still tally but different than open
                    FinalTissueRegionIndex = 3,
                    InDirectionOfFiberAxis = new Direction(0, 0, -1)
                }
            };
            _detectorInnerNA = new List<IDetectorInput>
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
                _source,
                _tissue,
                _detectorOuterOpen);
            _outputOuterOpen = new MonteCarloSimulation(input).Run();

            input = new SimulationInput(
                100,
                "",
                _simulationOptions,
                _source,
                _tissue,
                _detectorInnerOpen);
            _outputInnerOpen = new MonteCarloSimulation(input).Run();
            
            input = new SimulationInput(
                100,
                "",
                _simulationOptions,
                _source,
                _tissue,
                _detectorOuterNA);
            _outputOuterNA = new MonteCarloSimulation(input).Run();

            input = new SimulationInput(
                100,
                "",
                _simulationOptions,
                _source,
                _tissue,
                _detectorInnerNA);
            _outputInnerNA = new MonteCarloSimulation(input).Run();

        }

        /// <summary>
        /// Test to validate fiber at outer cylinder surface fully open with dot normal positive. 
        /// Validation values based on prior test.
        /// </summary>
        [Test]
        public void Validate_fully_open_internal_outer_fiber_detector_produces_correct_results()
        {
            Assert.Less(Math.Abs(_outputOuterOpen.IntSurFib - 0.071891), 0.000001);
            Assert.Less(Math.Abs(_outputOuterOpen.IntSurFib2 - 0.022480), 0.000001);
            Assert.AreEqual(23, _outputOuterOpen.IntSurFib_TallyCount);
        }

        /// <summary>
        /// Test to validate fiber at inner cylinder surface fully open with dot normal negative
        /// Validation values based on prior test.
        /// </summary>
        [Test]
        public void Validate_fully_open_internal_inner_fiber_detector_produces_correct_results()
        {
            Assert.Less(Math.Abs(_outputInnerOpen.IntSurFib - 0.009330), 0.000001);
            Assert.Less(Math.Abs(_outputInnerOpen.IntSurFib2 - 0.002902), 0.000001);
            Assert.AreEqual(3, _outputInnerOpen.IntSurFib_TallyCount);
        }

        /// <summary>
        /// Test to validate fiber at outer cylinder surface smaller NA with dot normal positive. 
        /// Validation values based on prior test.
        /// </summary>
        [Test]
        public void Validate_NA_internal_outer_fiber_detector_produces_correct_results()
        {
            // these results are smaller than open NA results for same fiber location as they should be
            Assert.Less(Math.Abs(_outputOuterNA.IntSurFib - 0.005993), 0.000001);
            Assert.Less(Math.Abs(_outputOuterNA.IntSurFib2 - 0.001801), 0.000001);
            Assert.AreEqual(2, _outputOuterNA.IntSurFib_TallyCount);
        }

        /// <summary>
        /// Test to validate fiber at inner cylinder surface smaller NA with dot normal positive. 
        /// Validation values based on prior test.
        /// </summary>
        [Test]
        public void Validate_NA_internal_inner_fiber_detector_produces_correct_results()
        {
            // these results are smaller than open NA results for same fiber location as they should be
            Assert.Less(Math.Abs(_outputInnerNA.IntSurFib - 0.006278), 0.000001);
            Assert.Less(Math.Abs(_outputInnerNA.IntSurFib2 - 0.001970), 0.000001);
            Assert.AreEqual(2, _outputInnerNA.IntSurFib_TallyCount);
        }
    }
}
