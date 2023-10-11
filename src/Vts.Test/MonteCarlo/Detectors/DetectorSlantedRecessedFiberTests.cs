using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Detectors
{
    /// <summary>
    /// These tests verify that the specification of a detector fiber processes the exiting photon
    /// correctly.  There are two intentions: (1) verify that a fiber centered at 0 produces same 
    /// results as R(rho) first bin with same radius, (2) compare fiber centered off 0 to R(rho)
    /// with rho radius at center of fiber, (3) verify Bargo's results
    /// </summary>
    [TestFixture]
    public class DetectorSlantedRecessedFiberTests
    {
        private SimulationOutput _outputNa;
        private SimulationOptions _simulationOptions;
        private ISourceInput _source;
        private ITissueInput _tissue;
        private IList<IDetectorInput> _detectorNa;
        private readonly double _detectorRadius = 1; // debug set to 10

        /// <summary>
        /// Setup input to the MC for a homogeneous one layer tissue with 
        /// fiber surface circle and specify fiber detector and R(rho).
        /// Need to create new simulation for open and NA cases since output
        /// cannot handle two detectors of same type
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
            _tissue = new MultiLayerTissueInput(
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 0.040), 
                        new OpticalProperties(0.038, 44, 0.9, 1.5)),
                    new LayerTissueRegion(
                        new DoubleRange(0.040, 0.110),
                        new OpticalProperties(0.038, 11, 0.9, 1.34)),
                    new LayerTissueRegion(
                        new DoubleRange(0.110, 1.0),
                        new OpticalProperties(0.095, 10, 0.95, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(1.0, 2.0),
                        new OpticalProperties(0.089, 2.4, 0.9, 1.36)),
                    new LayerTissueRegion(
                        new DoubleRange(2.0, 2.89),
                        new OpticalProperties(0.095, 10, 0.95, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(2.89, 2.96),
                        new OpticalProperties(0.038, 11, 0.9, 1.34)),
                    new LayerTissueRegion(
                        new DoubleRange(2.96, 3.0),
                        new OpticalProperties(0.038, 44, 0.9, 1.5)),
                    new LayerTissueRegion(
                        new DoubleRange(3.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                });           

            _detectorNa = new List<IDetectorInput>
            {
                new SlantedRecessedFiberDetectorInput()
                {
                    Radius = _detectorRadius,
                    Angle = Math.PI /6.0,
                    ZPlane = 0.0,
                    Center = new Position(1.0, 0, 0), 
                    NA = 0.39,
                    TallySecondMoment = true,
                    FinalTissueRegionIndex = 0
                },
                new ROfRhoDetectorInput() // 1mm wide ring to match fiber and 2 because beyond goes into 2nd
                {
                    Rho = new DoubleRange(0.0, 2 * _detectorRadius, 3),                    
                    // since tissue w fiber specified -> photon will be in 3 upon exit
                    FinalTissueRegionIndex = 0,
                    NA = 1.4,
                    TallySecondMoment = true
                },            
            };          

            var inputNa = new SimulationInput(
                1000,
                "",
                _simulationOptions,
                _source,
                _tissue,
                _detectorNa);
            _outputNa = new MonteCarloSimulation(inputNa).Run();
                        
        }

        /// <summary>
        /// Test to validate fiber at tissue surface fully open. 
        /// Validation values based on prior test.
        /// </summary>
        [Test]
        public void Validate_fully_open_surface_fiber_detector_produces_correct_results()
        {
            Assert.Less(Math.Abs(_outputNa.SlantedFib - 0.017095), 0.000001);
        }        
    }
}
