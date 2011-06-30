using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.PostProcessing;
using Vts.MonteCarlo.PhotonData;

namespace Vts.Test.MonteCarlo.Detectors
{
    // NOTE: these tests won't pass until pMCController and pMC detectors ContainsPoint figured out
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
        private SimulationInput _referenceInputOneLayerTissue;
        private Output _referenceOutputOneLayerTissue;
        private SimulationInput _referenceInputTwoLayerTissue;
        private Output _referenceOutputTwoLayerTissue;
        private double _layerThickness = 1.0;
        private double _factor;
        private pMCDatabase _databaseOneLayerTissue;
        private pMCDatabase _databaseTwoLayerTissue;

        /// <summary>
    /// These tests execute perturbation Monte Carlo (pMC) on a discrete absorption weighting (DAW)
    /// MC simulation with 100 photons and verify that 1) on-the-fly and pMC produces same results, and
    /// 2) the tally results match the linux results given the same seed
    /// mersenne twister STANDARD_TEST.  The linux results assumes photon passes
    /// through specular and deweights photon by specular.  This test starts photon 
    /// inside tissue and then multiplies result by specular deweighting to match 
    /// linux results.
        /// </summary>
        [TestFixtureSetUp]
        public void execute_reference_Monte_Carlo()
        {
            // generate reference database for homogeneous and one layer tissue
            GenerateReferenceDatabases();
        }
        /// <summary>
        /// Test to validate that setting mua and mus to the reference values
        /// determines results equal to reference
        /// </summary>
        [Test]  
        public void validate_pMC_DAW_ROfRhoAndTime_zero_perturbation_one_layer_tissue()
        {
            var postProcessedOutput = 
                PhotonTerminationDatabasePostProcessor.GenerateOutput(   
                    new List<IpMCDetectorInput>()
                    {
                        new pMCROfRhoAndTimeDetectorInput(
                            new DoubleRange(0.0, 10.0, 101),
                            new DoubleRange(0.0, 1.0, 101),
                            // set perturbed ops to reference ops
                            new List<OpticalProperties>() { 
                                _referenceInputOneLayerTissue.TissueInput.Regions[0].RegionOP,
                                _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP,
                                _referenceInputOneLayerTissue.TissueInput.Regions[2].RegionOP},
                            new List<int>() { 1 })
                    },
                    false,
                    _databaseOneLayerTissue,
                    _referenceInputOneLayerTissue);

            // validation value obtained from reference non-pMC run
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_rt[0, 0] - 
                _referenceOutputOneLayerTissue.R_rt[0, 0]), 0.00000000001);
            // validation value obtained from linux run using above input and seeded the same
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_rt[0, 0] * _factor - 61.5238307), 0.0000001);
       }
        [Test]
        public void validate_pMC_DAW_ROfRho_zero_perturbation_one_layer_tissue()
        {            
            var postProcessedOutput =
                PhotonTerminationDatabasePostProcessor.GenerateOutput(
                    new List<IpMCDetectorInput>()
                    {
                        new pMCROfRhoDetectorInput(
                            new DoubleRange(0.0, 10, 101),
                            // set perturbed ops to reference ops
                            new List<OpticalProperties>() { 
                                _referenceInputOneLayerTissue.TissueInput.Regions[0].RegionOP,
                                _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP,
                                _referenceInputOneLayerTissue.TissueInput.Regions[2].RegionOP},
                            new List<int>() { 1 })
                    },
                    false,
                    _databaseOneLayerTissue,
                    _referenceInputOneLayerTissue);
            // validation value obtained from reference non-pMC run
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_r[0] - _referenceOutputOneLayerTissue.R_r[0]), 0.00000000001);
            // validation value obtained from linux run using above input and seeded the same
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_r[0] * _factor - 0.615238307), 0.000000001);
        }
        /// <summary>
        /// Test to validate that setting mua and mus to the reference values
        /// determines results equal to reference
        /// </summary>
        [Test]
        public void validate_pMC_DAW_ROfRhoAndTime_zero_perturbation_of_top_layer()
        {
            var postProcessedOutput =
                PhotonTerminationDatabasePostProcessor.GenerateOutput(
                    new List<IpMCDetectorInput>()
                    {
                        new pMCROfRhoAndTimeDetectorInput(
                            new DoubleRange(0.0, 10.0, 101),
                            new DoubleRange(0.0, 1.0, 101),
                            new List<OpticalProperties>() { // perturbed ops
                                _referenceInputTwoLayerTissue.TissueInput.Regions[0].RegionOP,
                                _referenceInputTwoLayerTissue.TissueInput.Regions[1].RegionOP,
                                _referenceInputTwoLayerTissue.TissueInput.Regions[2].RegionOP,
                                _referenceInputTwoLayerTissue.TissueInput.Regions[3].RegionOP},
                            new List<int>() { 1 })
                    },
                    false,
                    _databaseTwoLayerTissue,
                    _referenceInputTwoLayerTissue);
            // validation value obtained from reference results
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_rt[0, 0] - _referenceOutputTwoLayerTissue.R_rt[0, 0]), 0.00000000001);
            // validation value obtained from linux run using above input and seeded the same
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_rt[0, 0] * _factor - 61.5238307), 0.0000001);
        }
        /// <summary>
        /// Define SimulationInput to describe homogeneous and two layer tissue 
        /// </summary>
        /// <returns></returns>
        private void GenerateReferenceDatabases()
        {
            var simulationOptions = new SimulationOptions(
                0,
                RandomNumberGeneratorType.MersenneTwister,
                AbsorptionWeightingType.Discrete,
                PhaseFunctionType.HenyeyGreenstein,
                new List<DatabaseType>() { DatabaseType.PhotonExitDataPoints, DatabaseType.CollisionInfo },  // write histories 
                true,
                0);
            var sourceInput = new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    1);
            var detectorInputs = new List<IDetectorInput>()
                {
                    new ROfRhoDetectorInput(new DoubleRange(0.0, 10.0, 101)),
                    new ROfRhoAndTimeDetectorInput(
                        new DoubleRange(0.0, 10.0, 101),
                        new DoubleRange(0.0, 1.0, 101)),
                };
            _referenceInputOneLayerTissue = new SimulationInput(
                100,
                "",  // can't create folder in isolated storage
                simulationOptions,
                sourceInput,
                new MultiLayerTissueInput(
                    new List<ITissueRegion>
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                detectorInputs);
            _factor = 1.0 - Optics.Specular(
                            _referenceInputOneLayerTissue.TissueInput.Regions[0].RegionOP.N,
                            _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.N);
            _referenceOutputOneLayerTissue =  new MonteCarloSimulation(_referenceInputOneLayerTissue).Run();

            _databaseOneLayerTissue = pMCDatabase.FromFile("photonExitDatabase", "collisionInfoDatabase");

            _referenceInputTwoLayerTissue = new SimulationInput(
                100,
                "",
                simulationOptions,
                sourceInput,
                new MultiLayerTissueInput(
                    new List<ITissueRegion>
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, _layerThickness),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(_layerThickness, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                detectorInputs);
            _referenceOutputTwoLayerTissue = new MonteCarloSimulation(_referenceInputTwoLayerTissue).Run();
            _databaseTwoLayerTissue = pMCDatabase.FromFile("photonExitDatabase", "collisionInfoDatabase");

        }
    }
}

