using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
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
    /// MC simulation with 100 seeded photons and verify that 1) on-the-fly and pMC produces same results, and
    /// 2) the tally results match the linux results given the same seed
    /// mersenne twister STANDARD_TEST.  The linux results assumes photon passes
    /// through specular and deweights photon by specular.  This test starts photon 
    /// inside tissue and then multiplies result by specular deweighting to match 
    /// linux results. The linux results are generated using the post-processing code in 
    /// the g_post subdirectory.
    /// </summary>
    [TestFixture]
    public class pMCCAWOneLayerDetectorsTests
    {
        private SimulationInput _referenceInputOneLayerTissue;
        private SimulationOutput _referenceOutputOneLayerTissue;
        private double _factor;
        private pMCDatabase _databaseOneLayerTissue;

        private readonly List<string> _listOfTestGeneratedFiles = new()
        {            
            "DiffuseReflectanceDatabase", // name has no "test" prefix, it is generated by the code so name fixed
            "DiffuseReflectanceDatabase.txt",
            "CollisionInfoDatabase",
            "CollisionInfoDatabase.txt",
            "file.txt", // file that captures screen output of MC simulation
        };

        [OneTimeTearDown]
        public void Clear_folders_and_files()
        {
            // make sure databases generated from previous tests are deleted
            foreach (var file in _listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
        }
        /// <summary>
        /// Define SimulationInput to describe homogeneous one layer tissue 
        /// </summary>
        /// <returns></returns>
        /// 
        [OneTimeSetUp]
        public void Execute_Monte_Carlo()
        {
            // delete previously generated files
            Clear_folders_and_files();

            var simulationOptions = new SimulationOptions(
                0,
                RandomNumberGeneratorType.MersenneTwister,
                AbsorptionWeightingType.Continuous,
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
                new ROfRhoDetectorInput() { Rho=new DoubleRange(0.0, 10.0, 101)},
                new ROfRhoAndTimeDetectorInput() { Rho=new DoubleRange(0.0, 10.0, 101), Time=new DoubleRange(0.0, 1.0, 101)} ,                  
                new ROfFxDetectorInput() { Fx=new DoubleRange(0.0, 0.5, 11)},
                new ROfFxAndTimeDetectorInput() { Fx=new DoubleRange(0.0, 0.5, 11), Time=new DoubleRange(0.0, 1.0, 101)}                   
            };
            MultiLayerTissueInput ti = new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey1"),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey2"),
                        new LayerTissueRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey3")
                    }
                );
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());
            _referenceInputOneLayerTissue = new SimulationInput(
                100,
                "",  // can't create folder in isolated storage
                simulationOptions,
                sourceInput,
                ti,
                detectorInputs);
            _factor = 1.0 - Optics.Specular(
                            _referenceInputOneLayerTissue.TissueInput.Regions[0].RegionOP.N,
                            _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.N);
            _referenceOutputOneLayerTissue = new MonteCarloSimulation(_referenceInputOneLayerTissue).Run();

            _databaseOneLayerTissue = pMCDatabase.FromFile("DiffuseReflectanceDatabase", "CollisionInfoDatabase");
        }

        /// <summary>
        /// Test to validate that setting mua and mus to the reference values
        /// determines results equal to reference
        /// </summary>
        [Test]  
        public void Validate_pMC_CAW_ROfRhoAndTime_zero_perturbation_one_layer_tissue()
        {
            var postProcessor =  new PhotonDatabasePostProcessor( 
                VirtualBoundaryType.pMCDiffuseReflectance,
                new List<IDetectorInput>()
                {
                    new pMCROfRhoAndTimeDetectorInput()
                    {
                        Rho=new DoubleRange(0.0, 10.0, 101),
                        Time=new DoubleRange(0.0, 1.0, 101),
                        // set perturbed ops to reference ops
                        PerturbedOps=new List<OpticalProperties>() { 
                            _referenceInputOneLayerTissue.TissueInput.Regions[0].RegionOP,
                            _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP,
                            _referenceInputOneLayerTissue.TissueInput.Regions[2].RegionOP},
                        PerturbedRegionsIndices=new List<int>() { 1 }
                    }
                },
                _databaseOneLayerTissue,
                _referenceInputOneLayerTissue);
            var postProcessedOutput = postProcessor.Run();

            // validation value obtained from reference non-pMC run
            Assert.That(Math.Abs(postProcessedOutput.pMC_R_rt[0, 0] - 
                _referenceOutputOneLayerTissue.R_rt[0, 0]), Is.LessThan(0.00000000001));
            // validation value obtained from linux run using above input and seeded the same
            Assert.That(Math.Abs(postProcessedOutput.pMC_R_rt[0, 0] * _factor - 92.2411018), Is.LessThan(0.0000001));
       }
        [Test]
        public void Validate_pMC_CAW_ROfRho_zero_perturbation_one_layer_tissue()
        {            
            var postProcessor = new PhotonDatabasePostProcessor(
                VirtualBoundaryType.pMCDiffuseReflectance,
                new List<IDetectorInput>()
                {
                    new pMCROfRhoDetectorInput()
                    {
                        Rho=new DoubleRange(0.0, 10, 101),
                        // set perturbed ops to reference ops
                        PerturbedOps=new List<OpticalProperties>() { 
                            _referenceInputOneLayerTissue.TissueInput.Regions[0].RegionOP,
                            _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP,
                            _referenceInputOneLayerTissue.TissueInput.Regions[2].RegionOP
                        },
                        PerturbedRegionsIndices=new List<int>() { 1 }
                    }
                },
                _databaseOneLayerTissue,
                _referenceInputOneLayerTissue);
            var postProcessedOutput = postProcessor.Run();
            // validation value obtained from reference non-pMC run
            Assert.That(Math.Abs(postProcessedOutput.pMC_R_r[0] - _referenceOutputOneLayerTissue.R_r[0]), Is.LessThan(0.00000000001));
            // validation value obtained from linux run using above input and seeded the same
            Assert.That(Math.Abs(postProcessedOutput.pMC_R_r[0] * _factor - 0.922411018), Is.LessThan(0.000000001));
        }
        /// <summary>
        /// Test to validate that setting mua and mus to the perturbed values (mua*2, mus*1.1)
        /// determines results equal to linux results for R(rho)
        /// </summary>
        [Test]
        public void Validate_pMC_CAW_ROfRho_nonzero_perturbation_one_layer_tissue()
        {
            var postProcessor = new PhotonDatabasePostProcessor(
                VirtualBoundaryType.pMCDiffuseReflectance,
                new List<IDetectorInput>()
                    {
                        new pMCROfRhoDetectorInput()
                        {
                            Rho=new DoubleRange(0.0, 10, 101),
                            // set perturbed ops to reference ops
                            PerturbedOps=new List<OpticalProperties>()
                                {
                                    _referenceInputOneLayerTissue.TissueInput.Regions[0].RegionOP,
                                    new OpticalProperties(
                                        _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.Mua * 2,
                                        _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.Musp * 1.1,
                                        _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.G,
                                        _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.N),
                                    _referenceInputOneLayerTissue.TissueInput.Regions[2].RegionOP
                                },
                            PerturbedRegionsIndices = new List<int>() {1}
                         }
                    },
                _databaseOneLayerTissue,
                _referenceInputOneLayerTissue);
            var postProcessedOutput = postProcessor.Run();

            // validation value obtained from linux run using above input and seeded the same
            Assert.That(Math.Abs(postProcessedOutput.pMC_R_r[0] * _factor - 1.013156), Is.LessThan(0.000001));
        }
        /// <summary>
        /// Test to validate that calling dMC results in not a NaN
        /// </summary>
        [Test]
        public void Validate_dMC_CAW_dROfRhodMua_dROfRhodMus_produces_not_NaN_results()
        {
            var postProcessor = new PhotonDatabasePostProcessor(
                VirtualBoundaryType.pMCDiffuseReflectance,
                new List<IDetectorInput>()
                {
                    new dMCdROfRhodMuaDetectorInput()
                    {
                        Rho = new DoubleRange(0.0, 10, 11),
                        // set perturbed ops to reference ops
                        PerturbedOps = new List<OpticalProperties>()
                        {
                            _referenceInputOneLayerTissue.TissueInput.Regions[0].RegionOP,
                            new OpticalProperties(
                                _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.Mua,
                                _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.Musp,
                                _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.G,
                                _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.N),
                            _referenceInputOneLayerTissue.TissueInput.Regions[2].RegionOP
                        },
                        PerturbedRegionsIndices = new List<int>() {1}
                    },
                    new dMCdROfRhodMusDetectorInput()
                    {
                        Rho = new DoubleRange(0.0, 10, 11),
                        // set perturbed ops to reference ops
                        PerturbedOps = new List<OpticalProperties>()
                        {
                            _referenceInputOneLayerTissue.TissueInput.Regions[0].RegionOP,
                            new OpticalProperties(
                                _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.Mua,
                                _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.Musp,
                                _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.G,
                                _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.N),
                            _referenceInputOneLayerTissue.TissueInput.Regions[2].RegionOP
                        },
                        PerturbedRegionsIndices = new List<int>() {1}
                    }
                },
                _databaseOneLayerTissue,
                _referenceInputOneLayerTissue);
            var postProcessedOutput = postProcessor.Run();
            // validation value obtained from linux run using above input and seeded the same
            Assert.That(Math.Abs(postProcessedOutput.dMCdMua_R_r[0]), Is.Not.EqualTo(double.NaN));
            Assert.That(Math.Abs(postProcessedOutput.dMCdMus_R_r[0]), Is.Not.EqualTo(double.NaN));
        }
        /// <summary>
        /// Test to validate that setting mua and mus to the reference values
        /// determines results equal to reference
        /// </summary>
        [Test]
        public void Validate_pMC_CAW_ROfFxAndTime_zero_perturbation_one_layer_tissue()
        {
            var postProcessor = new PhotonDatabasePostProcessor(
                VirtualBoundaryType.pMCDiffuseReflectance,
                new List<IDetectorInput>()
                {
                    new pMCROfFxAndTimeDetectorInput()
                    {
                        Fx=new DoubleRange(0.0, 0.5, 11),
                        Time=new DoubleRange(0.0, 1.0, 101),
                        // set perturbed ops to reference ops
                        PerturbedOps=new List<OpticalProperties>() { 
                            _referenceInputOneLayerTissue.TissueInput.Regions[0].RegionOP,
                            _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP,
                            _referenceInputOneLayerTissue.TissueInput.Regions[2].RegionOP},
                        PerturbedRegionsIndices=new List<int>() { 1 }
                    }
                },
                _databaseOneLayerTissue,
                _referenceInputOneLayerTissue);
            var postProcessedOutput = postProcessor.Run();

            // validation value obtained from reference non-pMC run
            Assert.That(Math.Abs(postProcessedOutput.pMC_R_fxt[1, 0].Real -
                _referenceOutputOneLayerTissue.R_fxt[1, 0].Real), Is.LessThan(0.00000000001));
            Assert.That(Math.Abs(postProcessedOutput.pMC_R_fxt[1, 0].Imaginary -
                 _referenceOutputOneLayerTissue.R_fxt[1, 0].Imaginary), Is.LessThan(0.00000000001));
        }
        [Test]
        public void Validate_pMC_CAW_ROfFx_zero_perturbation_one_layer_tissue()
        {
            var postProcessor = new PhotonDatabasePostProcessor(
                VirtualBoundaryType.pMCDiffuseReflectance,
                new List<IDetectorInput>()
                {
                    new pMCROfFxDetectorInput()
                    {
                        Fx=new DoubleRange(0.0, 0.5, 11),
                        // set perturbed ops to reference ops
                        PerturbedOps=new List<OpticalProperties>() { 
                            _referenceInputOneLayerTissue.TissueInput.Regions[0].RegionOP,
                            _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP,
                            _referenceInputOneLayerTissue.TissueInput.Regions[2].RegionOP
                        },
                        PerturbedRegionsIndices=new List<int>() { 1 },
                        TallySecondMoment = true
                    }
                },
                _databaseOneLayerTissue,
                _referenceInputOneLayerTissue);
            var postProcessedOutput = postProcessor.Run();
            // validation value obtained from reference non-pMC run
            Assert.That(Math.Abs(postProcessedOutput.pMC_R_fx[0].Real - 
                _referenceOutputOneLayerTissue.R_fx[0].Real), Is.LessThan(0.00000000001));
            Assert.That(Math.Abs(postProcessedOutput.pMC_R_fx[0].Imaginary - 
                _referenceOutputOneLayerTissue.R_fx[0].Imaginary), Is.LessThan(0.00000000001));
            Assert.That(Math.Abs(postProcessedOutput.pMC_R_fx2[1].Real - 0.483629), Is.LessThan(0.000001));
            Assert.That(Math.Abs(postProcessedOutput.pMC_R_fx2[1].Imaginary - 0.0), Is.LessThan(0.000001)); // imag of 2nd moment is 0
   
        }
        /// <summary>
        /// Test to validate that setting mua and mus to the perturbed values (mua*2, mus*1.1)
        /// determines results equal to linux results for R(fx)
        /// </summary>
        [Test]
        public void Validate_pMC_CAW_ROfFx_nonzero_perturbation_one_layer_tissue()
        {
            var postProcessor = new PhotonDatabasePostProcessor(
                VirtualBoundaryType.pMCDiffuseReflectance,
                new List<IDetectorInput>()
                    {
                        new pMCROfFxDetectorInput()
                        {
                            Fx=new DoubleRange(0.0, 0.5, 11),
                            // set perturbed ops to reference ops
                            PerturbedOps=new List<OpticalProperties>()
                                {
                                    _referenceInputOneLayerTissue.TissueInput.Regions[0].RegionOP,
                                    new OpticalProperties(
                                        _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.Mua * 2,
                                        _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.Musp * 1.1,
                                        _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.G,
                                        _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.N),
                                    _referenceInputOneLayerTissue.TissueInput.Regions[2].RegionOP
                                },
                            PerturbedRegionsIndices = new List<int>() {1}
                         }
                    },
                _databaseOneLayerTissue,
                _referenceInputOneLayerTissue);
            var postProcessedOutput = postProcessor.Run();

            // validation value obtained from prior run
            Assert.That(Math.Abs(postProcessedOutput.pMC_R_fx[1].Real - 0.303789), Is.LessThan(0.000001));
            Assert.That(Math.Abs(postProcessedOutput.pMC_R_fx[1].Imaginary - 0.036982), Is.LessThan(0.000001));
        }
        /// <summary>
        /// Test to validate that calling dMC results in not a NaN
        /// </summary>
        [Test]
        public void Validate_dMC_CAW_dROfRhodMua_produces_not_NaN_results()
        {
            var postProcessor = new PhotonDatabasePostProcessor(
                VirtualBoundaryType.pMCDiffuseReflectance,
                new List<IDetectorInput>()
                {
                    new dMCdROfRhodMuaDetectorInput()
                    {
                        Rho = new DoubleRange(0.0, 10, 101),
                        // set perturbed ops to reference ops
                        PerturbedOps = new List<OpticalProperties>()
                        {
                            _referenceInputOneLayerTissue.TissueInput.Regions[0].RegionOP,
                            new OpticalProperties(
                                _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.Mua,
                                _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.Musp,
                                _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.G,
                                _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.N),
                            _referenceInputOneLayerTissue.TissueInput.Regions[2].RegionOP
                        },
                        PerturbedRegionsIndices = new List<int>() {1}
                    },
                    new dMCdROfRhodMusDetectorInput()
                    {
                        Rho = new DoubleRange(0.0, 10, 101),
                        // set perturbed ops to reference ops
                        PerturbedOps = new List<OpticalProperties>()
                        {
                            _referenceInputOneLayerTissue.TissueInput.Regions[0].RegionOP,
                            new OpticalProperties(
                                _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.Mua,
                                _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.Musp,
                                _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.G,
                                _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.N),
                            _referenceInputOneLayerTissue.TissueInput.Regions[2].RegionOP
                        },
                        PerturbedRegionsIndices = new List<int>() {1}
                    }
                },
                _databaseOneLayerTissue,
                _referenceInputOneLayerTissue);
            var postProcessedOutput = postProcessor.Run();
            // validation value obtained from linux run using above input and seeded the same
            Assert.That(Math.Abs(postProcessedOutput.dMCdMua_R_r[0]), Is.Not.EqualTo(double.NaN));
            Assert.That(Math.Abs(postProcessedOutput.dMCdMus_R_r[0]), Is.Not.EqualTo(double.NaN));
        }
    }
}
