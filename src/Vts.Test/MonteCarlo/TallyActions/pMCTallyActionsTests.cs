using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.PostProcessing;
using Vts.MonteCarlo.PhotonData;

namespace Vts.Test.MonteCarlo.TallyActions
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
        [Test]
        public void validate_DAW_ROfRhoAndTime_zero_perturbation()
        {
            var peh = PhotonTerminationDatabase.FromFile("pMC_photonBiographies");
            var postProcessedOutput = 
                PhotonTerminationDatabasePostProcessor.GenerateOutput(   
                    new pMCDetectorInput(
                        new List<TallyType>()
                        {
                            TallyType.pMuaMusInROfRhoAndTime,
                        },
                        // the following ranges need to match _referenceInput values
                        new DoubleRange(0.0, 10, 101), // rho
                        new DoubleRange(0.0, 10, 101),  // z
                        new DoubleRange(0.0, Math.PI / 2, 2), // angle
                        new DoubleRange(0.0, 10000, 101), // time
                        new DoubleRange(0.0, 1000, 21), // omega
                        new DoubleRange(-10.0, 10.0, 201), // x
                        new DoubleRange(-10.0, 10.0, 201), // y
                        new MultiLayerTissue(
                            _referenceHomogeneousInput.TissueInput.Regions,
                            _referenceHomogeneousInput.Options.AbsorptionWeightingType,
                            _referenceHomogeneousInput.Options.PhaseFunctionType),
                        new List<OpticalProperties>() {
                            _referenceHomogeneousInput.TissueInput.Regions[0].RegionOP,
                            _referenceHomogeneousInput.TissueInput.Regions[1].RegionOP,
                            _referenceHomogeneousInput.TissueInput.Regions[2].RegionOP},
                        new List<int>() { 1 }
                    ),
                    peh, 
                    _referenceHomogeneousOutput.Input,
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
                new CustomPointSourceInputOld(
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
                new DetectorInput(
                    new List<TallyType>()
                        {
                            TallyType.ROfRhoAndTime,
                        },
                    new DoubleRange(0.0, 10, 101), // rho
                    new DoubleRange(0.0, 10, 101),  // z
                    new DoubleRange(0.0, Math.PI / 2, 2), // angle
                    new DoubleRange(0.0, 10000, 101), // time
                    new DoubleRange(0.0, 1000, 21), // omega
                    new DoubleRange(-10.0, 10.0, 201), // x
                    new DoubleRange(-10.0, 10.0, 201) // y
                )
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
                new CustomPointSourceInputOld(
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
                new DetectorInput(
                    new List<TallyType>
                        {
                            TallyType.ROfRhoAndTime,
                        },
                    new DoubleRange(0.0, 10, 101), // rho
                    new DoubleRange(0.0, 10, 101),  // z
                    new DoubleRange(0.0, Math.PI / 2, 2), // angle
                    new DoubleRange(0.0, 10000, 101), // time
                    new DoubleRange(0.0, 1000, 21), // omega
                    new DoubleRange(-10.0, 10.0, 201), // x
                    new DoubleRange(-10.0, 10.0, 201) // y
                )
            );
        }
        private static Output GenerateReferenceOutput(SimulationInput input)
        {
            // the following execution writes database file to isolated storage
            return new MonteCarloSimulation(input).Run();
        }
    }
}

