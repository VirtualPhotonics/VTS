using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.PostProcessing;
using Vts.MonteCarlo.PhotonData;

namespace Vts.Test.MonteCarlo.Detectors
{
    /// <summary>
    /// These tests verify that the specification of a detector fiber processes the exiting photon correctly
    /// </summary>
    [TestFixture]
    public class DetectorFiberTests
    {
        SimulationOutput _outputWithInternalFiber, _outputWithOpenSurfaceFiber, _outputWithNA0p22SurfaceFiber;
        private double _detectorRadius = 1;
        private double _internalDetectorCenterZ = 0;
        private double _internalDetectorHeight = 2;
        private double _surfaceDetectorCenterZ = -1e-6;
        private double _surfaceDetectorHeight = 2e-6;

        /// <summary>
        /// Setup input to the MC for a homogeneous one layer tissue with fiber cylinder and specify fiber detector
        /// </summary>
        [OneTimeSetUp]
        public void execute_Monte_Carlo()
        {
            // instantiate common classes
            var simulationOptions = new SimulationOptions(
                0,
                RandomNumberGeneratorType.MersenneTwister,
                AbsorptionWeightingType.Discrete,
                PhaseFunctionType.HenyeyGreenstein,
                new List<DatabaseType>(), 
                false, // track statistics
                0.0, // RR threshold -> 0 = no RR performed
                0);
            var source = new DirectionalPointSourceInput(
                     new Position(0.0, 0.0, 0.0),
                     new Direction(0.0, 0.0, 1.0),
                     1); 
            var tissueWithInternalFiber = new SingleCylinderTissueInput(
                new CylinderTissueRegion(
                    new Position(0, 0,  _internalDetectorCenterZ), // center of cylinder
                    _detectorRadius,
                    _internalDetectorHeight,
                    new OpticalProperties(0.01, 1.0, 0.8, 1.4)
                ),
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
            var tissueWithSurfaceFiber = new SingleCylinderTissueInput(
                new CylinderTissueRegion(
                    new Position(0, 0, _surfaceDetectorCenterZ), // center of cylinder
                    _detectorRadius,
                    _surfaceDetectorHeight,
                    new OpticalProperties(0.01, 1.0, 0.8, 1.4)
                ),
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
            var detectorInternal =  new List<IDetectorInput>
                {
                    new CylindricalFiberDetectorInput()
                    {
                        Center = new Position(0, 0, _internalDetectorCenterZ), // needs to match tissue region
                        Radius = _detectorRadius, // needs to match tissue region
                        HeightZ = _internalDetectorHeight, // needs to match tissue region
                        N = 1.4,
                        NA = double.PositiveInfinity,
                        FinalTissueRegionIndex = 3 // same results if this is set to 1
                    },
                    new ROfRhoDetectorInput()
                    {
                        Rho = new DoubleRange(0.0, 10.0, 11), FinalTissueRegionIndex= 0
                    },
                };
            var detectorSurfaceOpen = new List<IDetectorInput>
            {
                new CylindricalFiberDetectorInput()
                {
                    Center = new Position(0, 0, _surfaceDetectorCenterZ), // needs to match tissue region
                    Radius = _detectorRadius, // needs to match tissue region
                    HeightZ = _surfaceDetectorHeight, // needs to match tissue region
                    N = 1.4,
                    FinalTissueRegionIndex = 3
                },
                new ROfRhoDetectorInput()
                {
                    Rho = new DoubleRange(0.0, 10.0, 11), FinalTissueRegionIndex= 0
                },

            };
            var detectorSurfaceNA0p22 = new List<IDetectorInput>
            {
                new CylindricalFiberDetectorInput()
                {
                    Center = new Position(0, 0, _surfaceDetectorCenterZ), // needs to match tissue region
                    Radius = _detectorRadius, // needs to match tissue region
                    HeightZ = _surfaceDetectorHeight, // needs to match tissue region
                    N = 1.4,
                    FinalTissueRegionIndex = 3,
                    NA = 0.22
                },
                new ROfRhoDetectorInput()
                {
                    Rho = new DoubleRange(0.0, 10.0, 11), FinalTissueRegionIndex= 0, NA = 0.22
                },

            };

            var _inputWithInternalFiber = new SimulationInput(
                100,
                "",
                simulationOptions,
                source,
                tissueWithInternalFiber,
                detectorInternal);
            _outputWithInternalFiber = new MonteCarloSimulation(_inputWithInternalFiber).Run();

            var _inputWithOpenSurfaceFiber = new SimulationInput(
                100,
                "",
                simulationOptions,
                source,
                tissueWithSurfaceFiber,
                detectorSurfaceOpen);
            _outputWithOpenSurfaceFiber = new MonteCarloSimulation(_inputWithOpenSurfaceFiber).Run();

            var _inputWithNA0p22SurfaceFiber = new SimulationInput(
                100,
                "",
                simulationOptions,
                source,
                tissueWithSurfaceFiber,
                detectorSurfaceNA0p22);
            _outputWithNA0p22SurfaceFiber = new MonteCarloSimulation(_inputWithNA0p22SurfaceFiber).Run();

        }

        /// <summary>
        /// Test to validate fiber internal to tissue which has OPs that match surrounding tissue.
        /// Validation values based on prior test.
        /// </summary>
        [Test]
        public void validate_internal_fiber_detector_with_matching_OPs_does_not_change_results()
        {
            Assert.Less(Math.Abs(_outputWithInternalFiber.R_r[1] - 0.016491), 0.000001);
            Assert.Less(Math.Abs(_outputWithInternalFiber.CylFib - 0.00), 0.000001);
            Assert.AreEqual(_outputWithInternalFiber.CylFib_TallyCount, 0);
        }
        /// <summary>
        /// Test to validate fiber at tissue surface fully open.
        /// Validation values based on prior test.
        /// </summary>
        [Test]
        public void validate_fully_open_surface_fiber_detector_produces_correct_results()
        {
            Assert.Less(Math.Abs(_outputWithOpenSurfaceFiber.R_r[1] - 0.003095), 0.000001);
            Assert.Less(Math.Abs(_outputWithOpenSurfaceFiber.CylFib - 0.004736), 0.000001);
            Assert.AreEqual(_outputWithOpenSurfaceFiber.CylFib_TallyCount, 3);
        }
        /// <summary>
        /// Test to validate fiber at tissue surface fully open.
        /// Validation values based on prior test.
        /// </summary>
        [Test]
        public void validate_NA0p22_surface_fiber_detector_produces_correct_results()
        {
            Assert.Less(Math.Abs(_outputWithNA0p22SurfaceFiber.R_r[1] - 0.00), 0.000001);
            Assert.Less(Math.Abs(_outputWithNA0p22SurfaceFiber.CylFib - 0.00), 0.000001);
            Assert.AreEqual(_outputWithOpenSurfaceFiber.CylFib_TallyCount, 0);
        }
    }
}
