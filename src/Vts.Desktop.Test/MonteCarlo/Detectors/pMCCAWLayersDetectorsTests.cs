using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Helpers;
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
    public class pMCCAWLayersDetectorsTests
    {
        private SimulationInput _referenceInputTwoLayerTissue;
        private SimulationOutput _referenceOutputTwoLayerTissue;
        private double _layerThickness = 1.0;
        private double _factor;
        private pMCDatabase _databaseTwoLayerTissue;

        /// <summary>
    /// These tests execute perturbation Monte Carlo (pMC) on a continuous absorption weighting (CAW)
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
            // make sure databases generated from previous tests are deleted
            if (File.Exists("pMCCAWLayers_DiffuseReflectanceDatabase.txt"))
            {
                File.Delete("pMCCAWLayers_DiffuseReflectanceDatabase.txt");
            }
            if (File.Exists("pMCCAWLayers_DiffuseReflectanceDatabase"))
            {
                File.Delete("pMCCAWLayers_DiffuseReflectanceDatabase");
            }
            if (File.Exists("pMCCAWLayers_CollisionInfoDatabase.txt"))
            {
                File.Delete("pMCCAWLayers_CollisionInfoDatabase.txt");
            }
            if (File.Exists("pMCCAWLayers_CollisionInfoDatabase"))
            {
                File.Delete("pMCCAWLayers_CollisionInfoDatabase");
            }

            // generate reference database for two layer tissue
            GenerateReferenceDatabase();
        }
        /// <summary>
        /// Define SimulationInput to describe two layer tissue 
        /// </summary>
        /// <returns></returns>
        private void GenerateReferenceDatabase()
        {
            var simulationOptions = new SimulationOptions(
                0,
                RandomNumberGeneratorType.MersenneTwister,
                AbsorptionWeightingType.Continuous,
                PhaseFunctionType.HenyeyGreenstein,
                new List<DatabaseType>() { DatabaseType.pMCDiffuseReflectance },
                false, // track statistics
                0.0, // RR threshold -> 0 = no RR performed
                0);
            var sourceInput = new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    1);
            var detectorInputs = new List<IDetectorInput>()
            {
                new ROfRhoDetectorInput() {Rho=new DoubleRange(0.0, 10.0, 101)},
                new ROfRhoAndTimeDetectorInput() {Rho=new DoubleRange(0.0, 10.0, 101),Time=new DoubleRange(0.0, 1.0, 101)}                   
            };

            _referenceInputTwoLayerTissue = new SimulationInput(
                100,
                "",
                simulationOptions,
                sourceInput,
                new MultiLayerTissueInput(
                    new ITissueRegion[]
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
            _factor = 1.0 - Optics.Specular(
                            _referenceInputTwoLayerTissue.TissueInput.Regions[0].RegionOP.N,
                            _referenceInputTwoLayerTissue.TissueInput.Regions[1].RegionOP.N);
            _referenceOutputTwoLayerTissue = new MonteCarloSimulation(_referenceInputTwoLayerTissue).Run();
            _databaseTwoLayerTissue = pMCDatabase.FromFile("DiffuseReflectanceDatabase", "CollisionInfoDatabase");

        }
 
        /// <summary>
        /// Test to validate that setting mua and mus to the reference values
        /// determines results equal to reference
        /// </summary>
        [Test]
        public void validate_pMC_CAW_ROfRhoAndTime_zero_perturbation_of_top_layer()
        {
            var postProcessor = new PhotonDatabasePostProcessor(
                VirtualBoundaryType.pMCDiffuseReflectance,
                new List<IDetectorInput>()
                {
                    new pMCROfRhoAndTimeDetectorInput()
                    {
                        Rho=new DoubleRange(0.0, 10.0, 101),
                        Time=new DoubleRange(0.0, 1.0, 101),
                        PerturbedOps=new List<OpticalProperties>() { // perturbed ops
                            _referenceInputTwoLayerTissue.TissueInput.Regions[0].RegionOP,
                            _referenceInputTwoLayerTissue.TissueInput.Regions[1].RegionOP,
                            _referenceInputTwoLayerTissue.TissueInput.Regions[2].RegionOP,
                            _referenceInputTwoLayerTissue.TissueInput.Regions[3].RegionOP},
                        PerturbedRegionsIndices=new List<int>() { 1 }
                    }
                },
                _databaseTwoLayerTissue,
                _referenceInputTwoLayerTissue);
            var postProcessedOutput = postProcessor.Run();
            // validation value obtained from reference results
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_rt[0, 0] - _referenceOutputTwoLayerTissue.R_rt[0, 0]), 0.00000000001);
            // validation value obtained from linux run using above input and seeded the same
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_rt[0, 0] * _factor - 92.2411018), 0.0000001);
        }

    }
}

