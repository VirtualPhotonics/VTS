using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.PostProcessing;
using Vts.MonteCarlo.PhotonData;

namespace Vts.Test.MonteCarlo.Detectors
{
    /// <summary>
    /// These tests executes a MC simulation with 100 photons and verify
    /// that the tally results match the linux results given the same seed.
    /// The linux results are generated using the post-processing code in 
    /// the g_post subdirectory.
    /// mersenne twister STANDARD_TEST
    /// </summary>
    [TestFixture]
    public class pMCTallyActionsTests
    {
        SimulationInput _referenceHomogeneousInput;
        Output _referenceHomogeneousOutput;
        // Output _PMCOutput;

        /// <summary>
        /// Setup input to the MC, SimulationInput, and execute MC
        /// </summary>
        [TestFixtureSetUp]
        public void execute_reference_Monte_Carlo()
        {
            _referenceHomogeneousInput = GenerateHomogeneousReferenceInput();
            _referenceHomogeneousOutput = GenerateReferenceOutput(_referenceHomogeneousInput);
        }
 
        /// <summary>
        /// Test to validate that setting mua and mus to the reference values
        /// determines results equal to reference
        /// </summary>
        //[Test]  // ckh 3/15/11 hold on this test until dc-110309-detector_output_tally_refactoring merge
        public void validate_DAW_ROfRhoAndTime_zero_perturbation()
        {
            var database = PhotonDatabase.FromFile("pMC_photonBiographies");
            var postProcessedOutput = 
                PhotonTerminationDatabasePostProcessor.GenerateOutput(   
                    new List<IpMCDetectorInput>()
                    {
                        new pMCROfRhoAndTimeDetectorInput()
                    },
                    database,
                    _referenceHomogeneousInput,
                    new List<OpticalProperties>() { // perturbed ops
                        _referenceHomogeneousInput.TissueInput.Regions[0].RegionOP,
                        _referenceHomogeneousInput.TissueInput.Regions[1].RegionOP,
                        _referenceHomogeneousInput.TissueInput.Regions[2].RegionOP},
                    new List<int>() { 1 } // perturbed region
                    );
            // validation value obtained from linux run using above input 
            // and seeded the same
            Assert.Less(Math.Abs(postProcessedOutput.R_rt[2, 0] - 0.000609121451), 0.00000000001);
        }
        /// <summary>
        /// Define SimulationInput to describe homogeneous media
        /// </summary>
        /// <returns></returns>
        private SimulationInput GenerateHomogeneousReferenceInput()
        {
            return new SimulationInput(
                100,
                "pMC",
                new SimulationOptions(
                    0, 
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    true,  // write histories 
                    0),
                new CustomPointSourceInput(
                    new Position(0, 0, 0),
                    new Direction(0, 0, 1),
                    new DoubleRange(0.0, 0, 1),
                    new DoubleRange(0.0, 0, 1)
                ),
                new MultiLayerTissueInput(
                    new List<ITissueRegion>
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 0.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 0.0, 1.0))
                    }
                ),
                new List<IDetectorInput>()
                {
                    new ROfRhoDetectorInput(new DoubleRange(0.0, 10, 101)),
                    new ROfRhoAndTimeDetectorInput(
                        new DoubleRange(0.0, 10, 101),
                        new DoubleRange(0.0, 10, 101)),
                }
            );
        }
        /// <summary>
        /// Define SimulationInput to define 2 layered media
        /// </summary>
        /// <returns></returns>
        private SimulationInput GenerateLayeredReferenceInput()
        {
            Double _layerThickness = 1.0;
            return new SimulationInput(
                100,
                "Output",
                new SimulationOptions(
                    0,
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    true,  // write histories 
                    0),
                new CustomPointSourceInput(
                    new Position(0, 0, 0),
                    new Direction(0, 0, 1),
                    new DoubleRange(0.0, 0, 1),
                    new DoubleRange(0.0, 0, 1)
                ),
                new MultiLayerTissueInput(
                    new List<ITissueRegion>
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(1e-10, 0.0, 0.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, _layerThickness),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(_layerThickness, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(1e-10, 0.0, 0.0, 1.0))
                    }
                ),
                new List<IDetectorInput>()
                {
                    new ROfRhoDetectorInput(new DoubleRange(0.0, 10, 101)),
                    new ROfRhoAndTimeDetectorInput(
                        new DoubleRange(0.0, 10, 101),
                        new DoubleRange(0.0, 10, 101)),
                });
      
        }
        private static Output GenerateReferenceOutput(SimulationInput input)
        {
            // the following execution writes database file to isolated storage
            return new MonteCarloSimulation(input).Run();
        }
    }
}

