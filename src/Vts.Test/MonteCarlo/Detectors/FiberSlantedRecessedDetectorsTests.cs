using NUnit.Framework;
using System;
using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Detectors
{
    /// <summary>
    /// These tests verify that the specification of a detector fiber processes the exiting photon correctly.  
    /// The output of the surface fiber is compared with the output of the slanted recessed fiber with angle = 0 
    /// </summary>
    [TestFixture]
    public class FiberSlantedRecessedDetectorsTests
    {
        private SimulationOutput _outputNormalRecessedFiber;
        private SimulationOutput _outputSlantedRecessedFiber;
        private SimulationOutput _outputSurfaceFiber;
        private SimulationOptions _simulationOptions;
        private ISourceInput _source;
        private ITissueInput _tissueForSlantedRecessedFiber;
        private ITissueInput _tissueForSurfaceFiber;
        private IList<IDetectorInput> _detectorNormal;
        private IList<IDetectorInput> _detectorSlanted;
        private const double DetectorRadius = 1; 

        /// <summary>
        /// Setup input to the MC for a homogeneous one layer tissue with 
        /// fiber surface circle 
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
            _source = new DirectionalCircularSourceInput(
                      0.39,
                      0.2,
                      0.0,
                      new FlatSourceProfile(),
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                new Position (-0.134, 0.0, 0.0),
                new PolarAzimuthalAngles(0.5235,0.0),
                0);
            //Tissue for Slanted Recessed Fiber
            _tissueForSlantedRecessedFiber = new MultiLayerTissueInput(
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
                });
            //Tissue for Surface Fiber
            _tissueForSurfaceFiber =
                new MultiLayerTissueInput( 
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

            _detectorNormal = new List<IDetectorInput>
            {
                new SlantedRecessedFiberDetectorInput
                {
                    Radius = DetectorRadius,
                    Angle = 0.0,
                    ZPlane = 0.0,
                    Center = new Position(1.0, 0, 0), 
                    NA = 0.0,
                    N = 1.4,
                    TallySecondMoment = true,
                    FinalTissueRegionIndex = 0,
                    Name = "SlantedRecessedFiber1"
                },                
                new SurfaceFiberDetectorInput
                {
                    Center = new Position(0, 0, 0),
                    Radius = DetectorRadius,
                    TallySecondMoment = true,
                    N = 1.4,
                    FinalTissueRegionIndex = 0,
                    NA = 0.0,
                    Name = "SurfaceFiber1"
                },
            };
            _detectorSlanted = new List<IDetectorInput>
            {
                new SlantedRecessedFiberDetectorInput
                {
                    Radius = DetectorRadius,
                    Angle = Math.PI / 6.0,
                    ZPlane = 0.0,
                    Center = new Position(1.0, 0, 0),
                    NA = 0.39,
                    N = 1.4,
                    TallySecondMoment = true,
                    FinalTissueRegionIndex = 0,
                    Name = "SlantedRecessedFiber2"
                },                
            };

            var inputNormalRecessedFiber = new SimulationInput(
                1000,
                "",
                _simulationOptions,
                _source,
                _tissueForSlantedRecessedFiber,
                _detectorNormal);
            _outputNormalRecessedFiber = new MonteCarloSimulation(inputNormalRecessedFiber).Run();

            var inputSurfaceFiber = new SimulationInput(
                1000,
                "",
                _simulationOptions,
                _source,
                _tissueForSurfaceFiber,
                _detectorNormal);
            _outputSurfaceFiber = new MonteCarloSimulation(inputSurfaceFiber).Run();

            var inputSlantedRecessedFiber = new SimulationInput(
                1000,
                "",
                _simulationOptions,
                _source,
                _tissueForSlantedRecessedFiber,
                _detectorSlanted);
            _outputSlantedRecessedFiber = new MonteCarloSimulation(inputSlantedRecessedFiber).Run();

        }

        /// <summary>
        /// Test to validate slanted recessed detector
        /// i. Slanted Recessed fiber (Angle = 0.0, NA = 0.0) = Surface Fiber. Compare
        ///  Mean, Second Moment and Tally count
        /// ii. Check reflectance of Slanted Recessed fiber for Angle = 30 deg
        /// </summary>
        [Test]
        public void Validate_slanted_fiber_detector_produces_correct_results()
        {
            // check Mean of perpendicular fiber
            Assert.That(Math.Abs(_outputSurfaceFiber.AllSurfaceFiberDetectorMeans[0] -
                                 _outputNormalRecessedFiber.AllSlantedRecessedFiberDetectorMeans[0]), Is.LessThan(0.000001));
            // check Second Moment of surface fiber with perpendicular recessed fiber
            Assert.That(Math.Abs(_outputSurfaceFiber.AllSurfaceFiberDetectorSecondMoments[0] -
                                 _outputNormalRecessedFiber.AllSlantedRecessedFiberDetectorSecondMoments[0]), Is.LessThan(0.000001));
            // check TallyCount of surface fiber with perpendicular recessed fiber
            Assert.That(Math.Abs(_outputSurfaceFiber.AllSurfaceFiberDetectorTallyCounts[0] - 
                                 _outputNormalRecessedFiber.AllSlantedRecessedFiberDetectorTallyCounts[0]), Is.LessThan(0.000001));
            // check slanted fiber with prior test value
            Assert.That(Math.Abs(_outputSlantedRecessedFiber.AllSlantedRecessedFiberDetectorMeans[0] - 0.00586779), Is.LessThan(0.000001));                     
        }        
    }
}
