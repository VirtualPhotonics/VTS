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
    /// These tests verify that the specification of a detector fiber processes the exiting photon
    /// correctly.  There are two intentions: (1) verify that a fiber centered at 0 produces same 
    /// results as R(rho) first bin with same radius, (2) compare fiber centered off 0 to R(rho)
    /// with rho radius at center of fiber, (3) verify Bargo's results
    /// </summary>
    [TestFixture]
    public class DetectorFiberTests
    {
        private SimulationOutput _outputOpen, _outputNa, _outputNaOffCenter;
        private SimulationOptions _simulationOptions;
        private ISourceInput _source;
        private ITissueInput _tissue;
        private IList<IDetectorInput> _detectorOpen, _detectorNa, _detectorNaOffCenter;
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
            _source = new DirectionalPointSourceInput(
                     new Position(0.0, 0.0, 0.0),
                     new Direction(0.0, 0.0, 1.0),
                     1);

            _tissue =
                new MultiLayerWithSurfaceFiberTissueInput(
                    new SurfaceFiberTissueRegion(
                        new Position(0, 0, 0),
                        _detectorRadius, // needs to match SurfaceFiberDetectorInput
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)
                    ),
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
            // tissue specification to verify MultiLayerWithSurfaceFiberTissue 
            // increases reflectance.  If use this tissue need to change FinalTissueRegion 
            // for ALL detectors to 0
            //new MultiLayerTissueInput(
            //    new ITissueRegion[]
            //    
            //        new LayerTissueRegion(
            //            new DoubleRange(double.NegativeInfinity, 0.0),
            //            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
            //        new LayerTissueRegion(
            //            new DoubleRange(0.0, 100.0),
            //            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
            //        new LayerTissueRegion(
            //            new DoubleRange(100.0, double.PositiveInfinity),
            //            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
            //    
            //)

            _detectorOpen = new List<IDetectorInput>
            {
                new SurfaceFiberDetectorInput()
                {
                    Center = new Position(0, 0, 0), 
                    Radius = _detectorRadius, 
                    TallySecondMoment = true,
                    N = 1.4,  
                    NA = 1.4,
                    FinalTissueRegionIndex = 3
                },
                new ROfRhoDetectorInput() // 1mm wide ring to match fiber and 2 because beyond goes into 2nd
                {
                    Rho = new DoubleRange(0.0, 2 * _detectorRadius, 3),                    
                    // since tissue w fiber specified -> photon will be in 3 upon exit
                    FinalTissueRegionIndex = 3,
                    NA = 1.4,
                    TallySecondMoment = true
                },
            };
            _detectorNa = new List<IDetectorInput>
            {
                new SurfaceFiberDetectorInput()
                {
                    Center = new Position(0, 0, 0), 
                    Radius = _detectorRadius, 
                    TallySecondMoment = true,
                    N = 1.4,
                    FinalTissueRegionIndex = 3,
                    NA = 0.39
                },
                new ROfRhoDetectorInput() // ring to match fiber detector
                {
                    Rho = new DoubleRange(0.0, 2 * _detectorRadius, 3),
                    // since tissue w fiber specified -> photon will be in 3 upon exit
                    FinalTissueRegionIndex = 3,  
                    NA = 0.39,
                    TallySecondMoment = true
                },
            };
            _detectorNaOffCenter = new List<IDetectorInput>
            {
                new SurfaceFiberDetectorInput()
                {
                    Center = new Position(_detectorRadius, 0, 0), // diam = [0, 2*radius]
                    Radius = _detectorRadius,
                    TallySecondMoment = true,
                    N = 1.4,
                    FinalTissueRegionIndex = 3,
                    NA = 1.4
                },
                new ROfRhoDetectorInput() // ring to match fiber detector
                {
                    // place 1st rho bin center at _detectorRadius with width = 2*radius
                    Rho = new DoubleRange(_detectorRadius / 2, 2 * _detectorRadius + _detectorRadius / 2, 3),  
                    // since tissue w fiber specified -> photon will be in 3 upon exit
                    FinalTissueRegionIndex = 3,
                    NA = 1.4,
                    TallySecondMoment = true
                },
            };
            var inputOpen = new SimulationInput(
                100,
                "",
                _simulationOptions,
                _source,
                _tissue,
                _detectorOpen);
            _outputOpen = new MonteCarloSimulation(inputOpen).Run();

            var inputNa = new SimulationInput(
                100,
                "",
                _simulationOptions,
                _source,
                _tissue,
                _detectorNa);
            _outputNa = new MonteCarloSimulation(inputNa).Run();

            var inputNaOffCenter = new SimulationInput(
                100,
                "",
                _simulationOptions,
                _source,
                _tissue,
                _detectorNaOffCenter);
            _outputNaOffCenter = new MonteCarloSimulation(inputNaOffCenter).Run();
        }

        /// <summary>
        /// Test to validate fiber at tissue surface fully open. 
        /// Validation values based on prior test.
        /// </summary>
        [Test]
        public void Validate_fully_open_surface_fiber_detector_produces_correct_results()
        {
            Assert.Less(Math.Abs(_outputOpen.SurFib - 0.079266), 0.000001);
            Assert.Less(Math.Abs(_outputOpen.SurFib - _outputOpen.R_r[0]), 0.000001);
            Assert.Less(Math.Abs(_outputOpen.SurFib2 - 0.024315), 0.000001);
            Assert.AreEqual(26, _outputOpen.SurFib_TallyCount);
            // output for Bargo comparison
            //var sd = Math.Sqrt((_outputOpen.SurFib2 -
            //    _outputOpen.SurFib * _outputOpen.SurFib) / 100)
            //var threeSigmaPos = _outputOpen.SurFib + 3 * sd
            //var threeSigmaNeg = _outputOpen.SurFib - 3 * sd
        }
        /// <summary>
        /// Test to validate fiber at tissue surface fully open. Validation values based on prior test.
        /// Theory [Bargo et al., AO 42(16) 2003] states (R_NA/R_Open)=(NA/ntiss)^2
        /// (0.39/1.4)^2=0.0776.  If run this unit test with N=10000
        /// taking means of both (R_NA/R_Open)=0.0873 
        /// [mean-3sig, mean+3sig]=[0.0050, 0.0077] -> contains (0.0056/0.0727) agreement
        /// but if take 3sigma range may agree
        /// </summary>
        [Test]
        public void Validate_NA_surface_fiber_detector_produces_correct_results()
        {
            Assert.Less(Math.Abs(_outputNa.SurFib - 0.003034), 0.000001);
            Assert.Less(Math.Abs(_outputNa.SurFib - _outputNa.R_r[0]), 0.000001);
            Assert.Less(Math.Abs(_outputNa.SurFib2 - 0.000920), 0.000001);
            Assert.AreEqual(1, _outputNa.SurFib_TallyCount);
            // output for Bargo comparison
            //var sd = Math.Sqrt((_outputNA.SurFib2 -
            //        _outputNA.SurFib * _outputNA.SurFib) / 100)
            //var threeSigmaPos = _outputNA.SurFib + 3 * sd
            //var threeSigmaNeg = _outputNA.SurFib - 3 * sd
        }
        /// <summary>
        /// Test to verify that the results using a fiber and R(rho) produce
        /// equivalent results within variance
        /// </summary>
        [Test]
        public void Validate_NA_surface_fiber_detector_off_center_produces_correct_results()
        {
            var sd = Math.Sqrt((_outputNaOffCenter.SurFib2 -
                _outputNaOffCenter.SurFib * _outputNaOffCenter.SurFib) / 100);
            var threeSigmaPos = _outputNaOffCenter.SurFib + 3 * sd;
            var threeSigmaNeg = _outputNaOffCenter.SurFib - 3 * sd;
            Assert.IsTrue(_outputNaOffCenter.R_r[0] < threeSigmaPos);
            Assert.IsTrue(_outputNaOffCenter.R_r[0] > threeSigmaNeg);
        }
    }
}
