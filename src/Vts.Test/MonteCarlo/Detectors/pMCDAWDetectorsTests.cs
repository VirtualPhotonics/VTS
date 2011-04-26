using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.PostProcessing;
using Vts.MonteCarlo.PhotonData;

namespace Vts.Test.MonteCarlo.Detectors
{
    /// <summary>
    /// These tests executes a MC simulation with 100 photons and verify
    /// that the tally results match either the reference results (no 
    /// perturbation) or the linux results given the same seed.
    /// The linux results are generated using the post-processing code in 
    /// the g_post subdirectory.
    /// mersenne twister STANDARD_TEST
    /// </summary>
    [TestFixture]
    public class pMCDAWDetectorsTests
    {
        private SimulationInput _referenceInput;
        private Output _referenceOutput;

        /// <summary>
        /// Setup input to the MC, SimulationInput, and execute MC
        /// </summary>
        [TestFixtureSetUp]
        public void execute_reference_Monte_Carlo()
        {
            _referenceOutput = GenerateHomogeneousReferenceOutput();
        }
        /// <summary>
        /// Test to validate that setting mua and mus to the reference values
        /// determines results equal to reference
        /// </summary>
        [Test]  
        public void validate_pMC_DAW_ROfRhoAndTime_zero_perturbation()
        {
            var database = pMCDatabase.FromFile("pMC_photonExitDatabase", "pMC_collisionInfoDatabase");
            var postProcessedOutput = 
                PhotonTerminationDatabasePostProcessor.GenerateOutput(   
                    new List<IpMCDetectorInput>()
                    {
                        new pMCROfRhoAndTimeDetectorInput(
                            new DoubleRange(0.0, 10, 101),
                            new DoubleRange(0.0, 1, 101),
                            // set perturbed ops to reference ops
                            new List<OpticalProperties>() { 
                                _referenceInput.TissueInput.Regions[0].RegionOP,
                                _referenceInput.TissueInput.Regions[1].RegionOP,
                                _referenceInput.TissueInput.Regions[2].RegionOP},
                            new List<int>() { 1 })
                    },
                    database,
                    _referenceInput
                    );
            // validation value obtained from reference non-pMC run
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_rt[0, 0] - _referenceOutput.R_rt[0, 0]), 0.00000000001);
            // validation value obtained from linux run using above input and seeded the same
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_rt[0, 0] - 61.5238307), 0.0000001);
       }
        [Test]
        public void validate_pMC_DAW_ROfRho_zero_perturbation()
        {
            var database = pMCDatabase.FromFile("pMC_photonExitDatabase", "pMC_collisionInfoDatabase");
            var postProcessedOutput =
                PhotonTerminationDatabasePostProcessor.GenerateOutput(
                    new List<IpMCDetectorInput>()
                    {
                        new pMCROfRhoDetectorInput(
                            new DoubleRange(0.0, 10, 101),
                            // set perturbed ops to reference ops
                            new List<OpticalProperties>() { 
                                _referenceInput.TissueInput.Regions[0].RegionOP,
                                _referenceInput.TissueInput.Regions[1].RegionOP,
                                _referenceInput.TissueInput.Regions[2].RegionOP},
                            new List<int>() { 1 })
                    },
                    database,
                    _referenceInput
                    );
            // validation value obtained from reference non-pMC run
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_r[0] - _referenceOutput.R_r[0]), 0.00000000001);
            // validation value obtained from linux run using above input and seeded the same
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_r[0] - 0.615238307), 0.000000001);
        }
        /// <summary>
        /// Define SimulationInput to describe homogeneous media
        /// </summary>
        /// <returns></returns>
        private Output GenerateHomogeneousReferenceOutput()
        {
            _referenceInput = new SimulationInput(
                100,
                "pMC",
                new SimulationOptions(
                    0,
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType>() { DatabaseType.PhotonExitDataPoints, DatabaseType.CollisionInfo },  // write histories 
                    0),
                new PointSourceCollimatedInput( // todo: replace input files to correspond to new source implementations
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
                            new DoubleRange(0.0, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 0.0, 1.0))
                    }
                ),
                new List<IDetectorInput>()
                {
                    new ROfRhoDetectorInput(new DoubleRange(0.0, 10, 101)),
                    new ROfRhoAndTimeDetectorInput(
                        new DoubleRange(0.0, 10, 101),
                        new DoubleRange(0.0, 1, 101)),
                }
            );
            return new MonteCarloSimulation(_referenceInput).Run();
        }
    }
}

