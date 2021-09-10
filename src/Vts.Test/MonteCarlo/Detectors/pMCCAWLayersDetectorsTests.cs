using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
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
    /// These tests execute perturbation Monte Carlo (pMC) on a continuous absorption weighting (CAW)
    /// MC simulation with a seeded 100 photons and verify that 1) on-the-fly and pMC produces same results
    /// (no perturbation), and 2) the tally results match the linux results given the same seed
    /// mersenne twister STANDARD_TEST same seed.  The linux results assumes photon passes
    /// through specular and deweights photon by specular.  This test starts photon 
    /// inside tissue and then multiplies result by specular deweighting to match 
    /// linux results. The linux results are generated using the post-processing code in 
    /// the g_post subdirectory.
    /// </summary>
    [TestFixture]
    public class pMCCAWLayersDetectorsTests
    {
        private SimulationInput _referenceInputTwoLayerTissue;
        private SimulationOutput _referenceOutputTwoLayerTissue;
        private double _layerThickness = 1.0;
        private double _factor;
        private pMCDatabase _databaseTwoLayerTissue;

        List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "DiffuseReflectanceDatabase", // name has no "test" prefix, it is generated by the code so name fixed
            "DiffuseReflectanceDatabase.txt",
            "CollisionInfoDatabase",
            "CollisionInfoDatabase.txt",
            "file.txt", // file that captures screen output of MC simulation
        };

        [OneTimeTearDown]
        public void clear_folders_and_files()
        {
            // make sure databases generated from previous tests are deleted
            foreach (var file in listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
        }
        /// <summary>
        /// Define SimulationInput to describe two layer tissue 
        /// </summary>
        /// <returns></returns>
        [OneTimeSetUp]
        public void execute_Monte_Carlo()
        {
            // delete previously generated files
            clear_folders_and_files();

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
                new ATotalDetectorInput() {TallySecondMoment = true},
                new ROfRhoDetectorInput() {Rho=new DoubleRange(0.0, 10.0, 101)},
                new ROfRhoAndTimeDetectorInput() {Rho=new DoubleRange(0.0, 10.0, 101),Time=new DoubleRange(0.0, 1.0, 101)},
                new ROfXAndYAndTimeAndSubregionDetectorInput() {
                    X=new DoubleRange(-5.0, 5.0, 11),
                    Y=new DoubleRange(-3.0, 3.0, 7),
                    Time=new DoubleRange(0, 0.05, 11) },
                new ROfXAndYAndTimeAndSubregionRecessedDetectorInput() {
                    X=new DoubleRange(-5.0, 5.0, 11),
                    Y=new DoubleRange(-3.0, 3.0, 7),
                    Time=new DoubleRange(0, 0.05, 11),
                    ZPlane = -1.0 }
            };

            _referenceInputTwoLayerTissue = new SimulationInput(
                100,
                "",
                simulationOptions,
                sourceInput,
                new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, _layerThickness),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(_layerThickness, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
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
        /// <summary>
        /// Test to validate that setting mua and mus to the reference values
        /// determines results equal to reference for (pMC)ROfXAndYAndTimeAndSubregion and
        /// (pMC)ROfXAndYAndTimeAndSubregionRecessed
        /// </summary>
        [Test]
        public void validate_pMC_CAW_ROfXAndYAndTimeAndSubregion_zero_perturbation_both_layers()
        {
            var postProcessor = new PhotonDatabasePostProcessor(
                VirtualBoundaryType.pMCDiffuseReflectance,
                new List<IDetectorInput>()
                {
                    new pMCROfXAndYAndTimeAndSubregionDetectorInput()
                    {
                        X=new DoubleRange(-5.0, 5.0, 11),
                        Y=new DoubleRange(-3.0, 3.0, 7),
                        Time=new DoubleRange(0.0, 0.05, 11),
                        PerturbedOps=new List<OpticalProperties>() { // perturbed ops
                            _referenceInputTwoLayerTissue.TissueInput.Regions[0].RegionOP,
                            _referenceInputTwoLayerTissue.TissueInput.Regions[1].RegionOP,
                            _referenceInputTwoLayerTissue.TissueInput.Regions[2].RegionOP,
                            _referenceInputTwoLayerTissue.TissueInput.Regions[3].RegionOP},
                        PerturbedRegionsIndices=new List<int>() { 1, 2 }
                    },
                    new pMCROfXAndYAndTimeAndSubregionRecessedDetectorInput()
                    {
                        X=new DoubleRange(-5.0, 5.0, 11),
                        Y=new DoubleRange(-3.0, 3.0, 7),
                        Time=new DoubleRange(0.0, 0.05, 11),
                        ZPlane=-1.0,
                        PerturbedOps=new List<OpticalProperties>() { // perturbed ops
                            _referenceInputTwoLayerTissue.TissueInput.Regions[0].RegionOP,
                            _referenceInputTwoLayerTissue.TissueInput.Regions[1].RegionOP,
                            _referenceInputTwoLayerTissue.TissueInput.Regions[2].RegionOP,
                            _referenceInputTwoLayerTissue.TissueInput.Regions[3].RegionOP},
                        PerturbedRegionsIndices=new List<int>() { 1, 2 }
                    }
                },
                _databaseTwoLayerTissue,
                _referenceInputTwoLayerTissue);
            var postProcessedOutput = postProcessor.Run();
            // validation value obtained from reference results
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_xyts[0, 0, 9, 1] - 
                                 _referenceOutputTwoLayerTissue.R_xyts[0, 0, 9, 1]), 0.00000000001);
            // recessed detector
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_xytsr[0, 0, 9, 1] -
                                 _referenceOutputTwoLayerTissue.R_xytsr[0, 0, 9, 1]), 0.00000000001);
        }

        /// <summary>
        /// Test to validate mua non-zero perturbation and time in layer results
        /// </summary>
        [Test]
        public void validate_pMC_CAW_ROfXAndYAndTimeAndSubregion_time_in_layer_results()
        {
            var postProcessor = new PhotonDatabasePostProcessor(
                VirtualBoundaryType.pMCDiffuseReflectance,
                new List<IDetectorInput>()
                {
                    new pMCROfXAndYAndTimeAndSubregionDetectorInput()
                    {
                        X=new DoubleRange(-5.0, 5.0, 11),
                        Y=new DoubleRange(-3.0, 3.0, 7),
                        Time=new DoubleRange(0.0, 0.05, 11),
                        PerturbedOps=new List<OpticalProperties>() { // perturbed ops
                            _referenceInputTwoLayerTissue.TissueInput.Regions[0].RegionOP,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4),
                            new OpticalProperties(0.1, 1.0, 0.8, 1.4),
                            _referenceInputTwoLayerTissue.TissueInput.Regions[3].RegionOP},
                        PerturbedRegionsIndices=new List<int>() { 1, 2 },
                    }
                },
                _databaseTwoLayerTissue,
                _referenceInputTwoLayerTissue);
            var postProcessedOutput = postProcessor.Run();
            // the following could be in different time bins because binned based on time in region
            // not total time
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_xyts[0, 1, 5, 1] - 0.012422), 0.000001);
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_xyts[0, 1, 9, 2] - 0.012422), 0.000001);
            // show that unperturbed results are not same
            Assert.IsTrue(Math.Abs(postProcessedOutput.pMC_R_xyts[0, 1, 5, 1] -
                                   _referenceOutputTwoLayerTissue.R_xyts[0, 1, 5, 1]) > 0.000001);
            Assert.IsTrue(Math.Abs(postProcessedOutput.pMC_R_xyts[0, 1, 9, 2] -
                                   _referenceOutputTwoLayerTissue.R_xyts[0, 1, 9, 2]) > 0.000001);
        }
        /// <summary>
        /// Test to validate mua non-zero perturbation and time in layer results
        /// </summary>
        [Test]
        public void validate_pMC_CAW_ATotal_zero_perturbation_in_both_layer_results()
        {
            var postProcessor = new PhotonDatabasePostProcessor(
                VirtualBoundaryType.pMCDiffuseReflectance,
                new List<IDetectorInput>()
                {
                    new pMCATotalDetectorInput()
                    {
                        PerturbedOps=new List<OpticalProperties>() { // perturbed ops
                            _referenceInputTwoLayerTissue.TissueInput.Regions[0].RegionOP,
                            _referenceInputTwoLayerTissue.TissueInput.Regions[1].RegionOP,
                            _referenceInputTwoLayerTissue.TissueInput.Regions[2].RegionOP,
                            _referenceInputTwoLayerTissue.TissueInput.Regions[3].RegionOP},
                        PerturbedRegionsIndices=new List<int>() { 1, 2 },
                        TallySecondMoment = true
                    }
                },
                _databaseTwoLayerTissue,
                _referenceInputTwoLayerTissue);
            var postProcessedOutput = postProcessor.Run();
            // the following two values do not agree with those values in CAWLayersDetectorTests
            // because the slab thickness=20mm and the on-the-fly results tally to absorbed
            // energy when the photon tranmits out the bottom of the slab
            Assert.Less(Math.Abs(postProcessedOutput.pMC_Atot - 0.290926), 0.000001);
            Assert.Less(Math.Abs(postProcessedOutput.pMC_Atot2 - 0.185483), 0.000001);
            // show that unperturbed results are not same for reason in above comment
            Assert.IsTrue(Math.Abs(postProcessedOutput.pMC_Atot -
                                   _referenceOutputTwoLayerTissue.Atot)> 0.000001);
            Assert.IsTrue(Math.Abs(postProcessedOutput.pMC_Atot2 -
                                   _referenceOutputTwoLayerTissue.Atot2)> 0.000001);
        }
    }
}

