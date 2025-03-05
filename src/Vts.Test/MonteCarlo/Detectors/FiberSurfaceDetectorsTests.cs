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
    /// These tests verify that the specification of a detector fiber processes the exiting photon
    /// correctly.  There are two intentions: (1) verify that a fiber centered at 0 produces same 
    /// results as R(rho) first bin with same radius, (2) compare fiber centered off 0 to R(rho)
    /// with rho radius at center of fiber, (3) verify Bargo's results
    /// </summary>
    [TestFixture]
    public class FiberSurfaceDetectorsTests
    {
        private SimulationOutput _output;
        private SimulationOptions _simulationOptions;
        private ISourceInput _source;
        private ITissueInput _tissue;
        private IList<IDetectorInput> _detectors;
        private const double DetectorRadius = 1; // debug set to 10

        /// <summary>
        /// Setup input to the MC for a homogeneous one layer tissue with 
        /// fiber surface circle and specify fiber detector and R(rho). 
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
                        DetectorRadius, // needs to match SurfaceFiberDetectorInput
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
            // using SimulationOutput AllSurfaceFiberDetectorMeans definition
            _detectors = new List<IDetectorInput>
            {
                new SurfaceFiberDetectorInput() // centered at (0,0,0) and open NA
                {
                    Center = new Position(0, 0, 0), 
                    Radius = DetectorRadius, 
                    TallySecondMoment = true,
                    N = 1.4,  
                    NA = 1.4,
                    FinalTissueRegionIndex = 3,
                    Name = "SurfaceFiber1" // open NA
                },
                new ROfRhoDetectorInput() // 1mm wide ring to match fiber and 2 because beyond goes into 2nd
                {
                    Rho = new DoubleRange(0.0, 2 * DetectorRadius, 3),                    
                    // since tissue w fiber specified -> photon will be in 3 upon exit
                    FinalTissueRegionIndex = 3,
                    NA = 1.4,
                    TallySecondMoment = true,
                    Name = "ROfRho1"
                },
                new SurfaceFiberDetectorInput() // centered at (0,0,0) and NA=0.39
                {
                    Center = new Position(0, 0, 0), 
                    Radius = DetectorRadius, 
                    TallySecondMoment = true,
                    N = 1.4,
                    FinalTissueRegionIndex = 3,
                    NA = 0.39,
                    Name = "SurfaceFiber2" // NA = 0.39
                },
                new ROfRhoDetectorInput() // ring to match fiber detector
                {
                    Rho = new DoubleRange(0.0, 2 * DetectorRadius, 3),
                    // since tissue w fiber specified -> photon will be in 3 upon exit
                    FinalTissueRegionIndex = 3,  
                    NA = 0.39,
                    TallySecondMoment = true,
                    Name = "ROfRho2"
                },
                new SurfaceFiberDetectorInput() // off center and open NA
                {
                    Center = new Position(DetectorRadius, 0, 0), // diam = 2*radius
                    Radius = DetectorRadius,
                    TallySecondMoment = true,
                    N = 1.4,
                    FinalTissueRegionIndex = 3,
                    NA = 1.4,
                    Name = "SurfaceFiber3"
                },
                new ROfRhoDetectorInput() // ring to match off center fiber detector
                {
                    // place 1st rho bins w/ center at _detectorRadius with width = 2*radius
                    Rho = new DoubleRange(0, 4 * DetectorRadius, 3),  
                    // since tissue w fiber specified -> photon will be in 3 upon exit
                    FinalTissueRegionIndex = 3,
                    NA = 1.4,
                    TallySecondMoment = true,
                    Name = "ROfRho3"
                },
            };
            var input = new SimulationInput(
                100,
                "",
                _simulationOptions,
                _source,
                _tissue,
                _detectors);
            _output = new MonteCarloSimulation(input).Run();
        }

        /// <summary>
        /// Test to validate fiber at tissue surface fully open. 
        /// Validation values based on prior test.  Final tests compare with Bargo's results.
        /// </summary>
        [Test]
        public void Validate_fully_open_surface_fiber_detector_produces_correct_results()
        {
            Assert.That(Math.Abs(_output.AllSurfaceFiberDetectorMeans[0] - 0.079266), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_output.AllSurfaceFiberDetectorMeans[0] -
                                 _output.AllROfRhoDetectorMeans[0][0]), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_output.AllSurfaceFiberDetectorSecondMoments[0] - 0.024315), Is.LessThan(0.000001));
            Assert.That(_output.AllSurfaceFiberDetectorTallyCounts[0], Is.EqualTo(26));
            // output for Bargo comparison
            var sd = Math.Sqrt((_output.AllSurfaceFiberDetectorSecondMoments[0] -
                                _output.AllSurfaceFiberDetectorMeans[0] * _output.AllSurfaceFiberDetectorMeans[0]) /
                               100);
            var threeSigmaPos = _output.AllSurfaceFiberDetectorMeans[0] + 3 * sd;
            var threeSigmaNeg = _output.AllSurfaceFiberDetectorMeans[0] - 3 * sd; 
            Assert.That(_output.AllSurfaceFiberDetectorMeans[0] < threeSigmaPos, Is.True);
            Assert.That(_output.AllSurfaceFiberDetectorMeans[0] > threeSigmaNeg, Is.True);
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
            Assert.That(Math.Abs(_output.AllSurfaceFiberDetectorMeans[1] - 0.003034), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_output.AllSurfaceFiberDetectorMeans[1] -
                                 _output.AllROfRhoDetectorMeans[1][0]), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_output.AllSurfaceFiberDetectorSecondMoments[1] - 0.000920), Is.LessThan(0.000001));
            Assert.That(_output.AllSurfaceFiberDetectorTallyCounts[1], Is.EqualTo(1));
            // output for Bargo comparison
            var sd = Math.Sqrt((_output.AllSurfaceFiberDetectorSecondMoments[1] - 
                _output.AllSurfaceFiberDetectorMeans[1] * _output.AllSurfaceFiberDetectorMeans[1]) / 100);
            var threeSigmaPos = _output.AllSurfaceFiberDetectorMeans[1] + 3 * sd;
            var threeSigmaNeg = _output.AllSurfaceFiberDetectorMeans[1] - 3 * sd;
            Assert.That(_output.AllSurfaceFiberDetectorMeans[1] < threeSigmaPos, Is.True);
            Assert.That(_output.AllSurfaceFiberDetectorMeans[1] > threeSigmaNeg, Is.True);
        }

        /// <summary>
        /// Test to verify that the results using a fiber and R(rho) produce
        /// equivalent results within variance
        /// </summary>
        [Test]
        public void Validate_NA_surface_fiber_detector_off_center_produces_correct_results()
        {
            var sd = Math.Sqrt((_output.AllSurfaceFiberDetectorSecondMoments[2] -
                _output.AllSurfaceFiberDetectorMeans[2] * _output.AllSurfaceFiberDetectorMeans[2]) / 100);
            var threeSigmaPos = _output.AllSurfaceFiberDetectorMeans[2] + 3 * sd;
            var threeSigmaNeg = _output.AllSurfaceFiberDetectorMeans[2] - 3 * sd;
            Assert.That(_output.AllROfRhoDetectorMeans[2][0] < threeSigmaPos, Is.True);
            Assert.That(_output.AllROfRhoDetectorMeans[2][0] > threeSigmaNeg, Is.True);
        }
    }
}
