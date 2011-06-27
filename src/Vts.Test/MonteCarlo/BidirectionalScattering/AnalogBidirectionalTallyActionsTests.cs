using System;
using System.Numerics;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.BidirectionalScattering
{
    /// <summary>
    /// These tests execute an Analog MC bidirectional simulation with 1e6 photons and verify
    /// that the tally results match the analytic solution within variance
    /// mersenne twister STANDARD_TEST.
    /// These solutions assume index matched slab.
    /// </summary>
    [TestFixture]
    public class AnalogBidirectionalTallyActionsTests
    {
        Output _output;
        Double _slabThickness = 10;
        Double _mua = 0.01;
        Double _musp = 0.198;  // mus = 0.99
        Double _g = 0.8;

        /// <summary>
        /// Setup input to the MC, SimulationInput, and execute MC
        /// </summary>
        [TestFixtureSetUp]
        public void execute_Monte_Carlo()
        {
            var input = new SimulationInput(
                10000, // number needed to get enough photons to Td 
                "Output",
                new SimulationOptions(
                    0, 
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Analog, 
                    PhaseFunctionType.Bidirectional,
                    null, 
                    true,
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0                   
                ),
                new MultiLayerTissueInput(
                    new List<ITissueRegion>
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 0.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, _slabThickness),
                            new OpticalProperties(_mua, _musp, _g, 1.0)), // index matched slab
                        new LayerRegion(
                            new DoubleRange(_slabThickness, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 0.0, 1.0))
                    }
                ),
                new List<IDetectorInput>()
                {
                    new RDiffuseDetectorInput(),
                    new TDiffuseDetectorInput(),
                    new ATotalDetectorInput()
                }
            );
            _output = new MonteCarloSimulation(input).Run();
        }

        // todo: add analytic variance and use this for error bounds

        // Diffuse Reflectance
        [Test]
        public void validate_bidirectional_analog_RDiffuse()
        {
            var analyticSolution = BidirectionalAnalyticSolutions.GetBidirectionalRadianceInSlab(
                _slabThickness,
                new OpticalProperties(_mua, _musp, _g, 1.0),
                -1, // direction -1=up
                0); // position at surface

            Assert.Less(Math.Abs(_output.Rd - analyticSolution), 0.01);
        }
        // Total Absorption
        [Test]
        public void validate_bidirectional_analog_ATotal()
        {
            var analyticSolutionRight =
                BidirectionalAnalyticSolutions.GetBidirectionalRadianceIntegratedOverInterval(
                _slabThickness,
                new OpticalProperties(_mua, _musp, _g, 1.0),
                1,
                0,
                _slabThickness);
            var analyticSolutionLeft = 
                BidirectionalAnalyticSolutions.GetBidirectionalRadianceIntegratedOverInterval(
                _slabThickness,
                new OpticalProperties(_mua, _musp, _g, 1.0),
                -1,
                0,
                _slabThickness);
            var analyticSolution = analyticSolutionRight - analyticSolutionLeft; // directional net
            Assert.Less(Math.Abs(_output.Atot - _mua * analyticSolution), 0.01);
        }
        // Diffuse Transmittance
        [Test]
        public void validate_bidirectional_analog_TDiffuse()
        {
            var analyticSolution = BidirectionalAnalyticSolutions.GetBidirectionalRadianceInSlab(
                _slabThickness,
                new OpticalProperties(_mua, _musp, _g, 1.0),
                1, // direction 1=down
                _slabThickness); // position at slab end

            Assert.Less(Math.Abs(_output.Td - analyticSolution), 0.01);
        }
        // with no refractive index mismatch, Rd + Atot + Td should equal 1
        [Test]
        public void validate_bidirectional_analog_detector_sum_equals_one()
        {
            Assert.Less(Math.Abs(_output.Rd + _output.Atot + _output.Td - 1.0), 0.1);
        }
    }
}
