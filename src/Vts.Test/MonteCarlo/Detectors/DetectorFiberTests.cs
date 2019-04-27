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
        private SimulationInput _inputWithInternalFiber, _inputWithSurfaceFiber;
        private SimulationOutput _outputWithInternalFiber, _outputWithSurfaceFiber;

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
                     3); 
            var tissueWithInternalFiber = new SingleCylinderTissueInput(
                new CylinderTissueRegion(
                    new Position(1, 0, 1), // center of cylinder
                    0.6,
                    2.0,
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
                    new Position(0, 0, 1e-6), // center of cylinder
                    0.6,
                    0.0,
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
                        Center = new Position(1, 0, 1), // needs to match tissue region
                        Radius = 0.6, // needs to match tissue region
                        HeightZ = 2.0, // needs to match tissue region
                        NA = double.PositiveInfinity,
                        FinalTissueRegionIndex = 1
                    },
                    new ROfRhoDetectorInput()
                    {
                        Rho = new DoubleRange(0.0, 10.0, 11), FinalTissueRegionIndex= 0
                    },
                };
            var detectorSurface = new List<IDetectorInput>
            {
                new CylindricalFiberDetectorInput()
                {
                    Center = new Position(0, 0, 0), // needs to match tissue region
                    Radius = 0.6, // needs to match tissue region
                    HeightZ = 0.0, // needs to match tissue region
                    NA = 0.22,
                    FinalTissueRegionIndex = 1
                },
                new ROfRhoDetectorInput()
                {
                    Rho = new DoubleRange(0.0, 10.0, 11), FinalTissueRegionIndex= 0
                },

            };
            //var _inputWithSurfaceFiber = new SimulationInput(
            //    100,
            //    "",
            //    simulationOptions,
            //    source,
            //    tissueWithSurfaceFiber,
            //    detectorSurface);
            //_outputWithSurfaceFiber = new MonteCarloSimulation(_inputWithSurfaceFiber).Run();

            var _inputWithInternalFiber = new SimulationInput(
                100,
                "",
                simulationOptions,
                source,
                tissueWithInternalFiber,
                detectorInternal);
            _outputWithInternalFiber = new MonteCarloSimulation(_inputWithInternalFiber).Run();

        }

        /// <summary>
        /// Test to validate fiber internal to tissue which has OPs that match surrounding tissue.
        /// Validation values based on prior test.
        /// </summary>
        //[Test]
        //public void validate_internal_fiber_detector_with_matching_OPs_does_not_change_results()
        //{
        //    Assert.Less(Math.Abs(_outputWithInternalFiber.R_r[2] - 0.000610), 0.000001);
        //    Assert.AreEqual(_outputWithInternalFiber.CylFib, 0.0);
        //}
        /// <summary>
        /// Test to validate fiber at tissue surface.
        /// Validation values based on prior test.
        /// </summary>
        //[Test]
        //public void validate_surface_fiber_detector_produces_correct_results()
        //{
        //    Assert.Less(Math.Abs(_outputWithSurfaceFiber.R_r[2] - 0.000610), 0.000001);
        //    Assert.AreEqual(_outputWithSurfaceFiber.CylFib, 0.0);
        //}
    }
}
