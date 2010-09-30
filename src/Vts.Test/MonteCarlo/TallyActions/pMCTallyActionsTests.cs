using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.TallyActions
{
    /// <summary>
    /// These tests executes a MC simulation with 100 photons and verify
    /// that the tally results match the linux results given the same seed
    /// mersenne twister STANDARD_TEST
    /// </summary>
    [TestFixture]
    public class pMCTallyActionsTests
    {
        Output _onTheFlyOutput;
        Output _PMCOutput;
        Double _layerThickness = 1.0;

        [TestFixtureSetUp]
        public void execute_Monte_Carlo()
        {
           var input = new SimulationInput(
                100,
                "Output",
                new PointSourceInput(
                    new Position(0, 0, 0),
                    new Direction(0, 0, 1),
                    new DoubleRange(0.0, 0, 1),
                    new DoubleRange(0.0, 0, 1)
                ),
                new MultiLayerTissueInput(
                    new List<LayerRegion>
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0, 2),
                            new OpticalProperties(1e-10, 0.0, 0.0, 1.0),
                            AbsorptionWeightingType.Discrete),
                        new LayerRegion(
                            new DoubleRange(0.0, _layerThickness, 2),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                            AbsorptionWeightingType.Discrete),
                        new LayerRegion(
                            new DoubleRange(_layerThickness, 100.0, 2),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                            AbsorptionWeightingType.Discrete),
                        new LayerRegion(
                            new DoubleRange(100.0, double.PositiveInfinity, 2),
                            new OpticalProperties(1e-10, 0.0, 0.0, 1.0),
                            AbsorptionWeightingType.Discrete)
                    }
                ),
                new DetectorInput(
                    new List<TallyType>()
                        {
                            TallyType.pMuaMusInROfRho,
                            TallyType.pMuaMusInROfRhoAndTime,
                        },
                    new DoubleRange(0.0, 10, 101), // rho
                    new DoubleRange(0.0, 10, 101),  // z
                    new DoubleRange(0.0, Math.PI / 2, 1), // angle
                    new DoubleRange(0.0, 10000, 101), // time
                    new DoubleRange(0.0, 1000, 21), // omega
                    new DoubleRange(-10.0, 10.0, 201), // x
                    new DoubleRange(-10.0, 10.0, 201) // y
                )
            );
            SimulationOptions options = new SimulationOptions(
                0, 
                RandomNumberGeneratorType.MersenneTwister,
                AbsorptionWeightingType.Discrete, 
                false, 
                false, 
                true,  // write histories 
                false, 
                0);
            _onTheFlyOutput = new MonteCarloSimulation(input, options).Run();
        }

        // validation values obtained from linux run using above input and seeded the same
        //
        [Test]
        public void validate_zero_perturbation()
        {
            //Assert.Less(Math.Abs(_output.Rd - 0.565765638), 0.000000001);
        }

    }
}
