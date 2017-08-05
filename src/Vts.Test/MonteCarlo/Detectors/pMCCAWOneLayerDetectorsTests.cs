using System;
using System.IO;
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
    /// These tests executes a MC simulation with 100 photons and verify
    /// that the tally results match either the reference results (no 
    /// perturbation) or the linux results given the same seed.
    /// The linux results are generated using the post-processing code in 
    /// the g_post subdirectory.
    /// mersenne twister STANDARD_TEST
    /// </summary>
    [TestFixture]
    public class pMCCAWOneLayerDetectorsTests
    {
        private SimulationInput _referenceInputOneLayerTissue;
        private SimulationOutput _referenceOutputOneLayerTissue;
        private double _layerThickness = 1.0;
        private double _factor;
        private pMCDatabase _databaseOneLayerTissue;

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
            if (File.Exists("pMCCAWOneLayer_DiffuseReflectanceDatabase.txt"))
            {
                File.Delete("pMCCAWOneLayer_DiffuseReflectanceDatabase.txt");
            }
            if (File.Exists("pMCCAWOneLayer_DiffuseReflectanceDatabase"))
            {
                File.Delete("pMCCAWOneLayer_DiffuseReflectanceDatabase");
            }
            if (File.Exists("pMCCAWOneLayer_CollisionInfoDatabase.txt"))
            {
                File.Delete("pMCCAWOneLayer_CollisionInfoDatabase.txt");
            }
            if (File.Exists("pMCCAWOneLayer_CollisionInfoDatabase"))
            {
                File.Delete("pMCCAWOneLayer_CollisionInfoDatabase");
            }

            // generate reference database for homogeneous tissue
            GenerateReferenceDatabase();
        }
        /// <summary>
        /// Define SimulationInput to describe homogeneous one layer tissue 
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
                new ROfRhoDetectorInput() { Rho=new DoubleRange(0.0, 10.0, 101)},
                new ROfRhoAndTimeDetectorInput() { Rho=new DoubleRange(0.0, 10.0, 101), Time=new DoubleRange(0.0, 1.0, 101)} ,                  
                new ROfFxDetectorInput() { Fx=new DoubleRange(0.0, 0.5, 11)},
                new ROfFxAndTimeDetectorInput() { Fx=new DoubleRange(0.0, 0.5, 11), Time=new DoubleRange(0.0, 1.0, 101)}                   
            };
            _referenceInputOneLayerTissue = new SimulationInput(
                100,
                "",  // can't create folder in isolated storage
                simulationOptions,
                sourceInput,
                new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
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
        public void validate_pMC_CAW_ROfRhoAndTime_zero_perturbation_one_layer_tissue()
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
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_rt[0, 0] - 
                _referenceOutputOneLayerTissue.R_rt[0, 0]), 0.00000000001);
            // validation value obtained from linux run using above input and seeded the same
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_rt[0, 0] * _factor - 92.2411018), 0.0000001);
       }
        [Test]
        public void validate_pMC_CAW_ROfRho_zero_perturbation_one_layer_tissue()
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
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_r[0] - _referenceOutputOneLayerTissue.R_r[0]), 0.00000000001);
            // validation value obtained from linux run using above input and seeded the same
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_r[0] * _factor - 0.922411018), 0.000000001);
        }
        /// <summary>
        /// Test to validate that setting mua and mus to the perturbed values (mua*2, mus*1.1)
        /// determines results equal to linux results for R(rho)
        /// </summary>
        [Test]
        public void validate_pMC_CAW_ROfRho_nonzero_perturbation_one_layer_tissue()
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
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_r[0] * _factor - 1.013156), 0.000001);
        }
        /// <summary>
        /// Test to validate that calling dMC results in not a NaN
        /// </summary>
        [Test]
        public void validate_dMC_CAW_dROfRhodMua_produces_not_NaN_results()
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
            Assert.AreNotEqual(Math.Abs(postProcessedOutput.dMCdMua_R_r[0]), double.NaN);
            Assert.AreNotEqual(Math.Abs(postProcessedOutput.dMCdMus_R_r[0]), double.NaN);
        }
        /// <summary>
        /// Test to validate that setting mua and mus to the reference values
        /// determines results equal to reference
        /// </summary>
        [Test]
        public void validate_pMC_CAW_ROfFxAndTime_zero_perturbation_one_layer_tissue()
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
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_fxt[1, 0].Real -
                _referenceOutputOneLayerTissue.R_fxt[1, 0].Real), 0.00000000001);
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_fxt[1, 0].Imaginary -
                 _referenceOutputOneLayerTissue.R_fxt[1, 0].Imaginary), 0.00000000001);
        }
        [Test]
        public void validate_pMC_CAW_ROfFx_zero_perturbation_one_layer_tissue()
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
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_fx[0].Real - 
                _referenceOutputOneLayerTissue.R_fx[0].Real), 0.00000000001);
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_fx[0].Imaginary - 
                _referenceOutputOneLayerTissue.R_fx[0].Imaginary), 0.00000000001);
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_fx2[1].Real - 0.483629), 0.000001);
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_fx2[1].Imaginary - 0.0), 0.000001); // imag of 2nd moment is 0
   
        }
        /// <summary>
        /// Test to validate that setting mua and mus to the perturbed values (mua*2, mus*1.1)
        /// determines results equal to linux results for R(fx)
        /// </summary>
        [Test]
        public void validate_pMC_CAW_ROfFx_nonzero_perturbation_one_layer_tissue()
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
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_fx[1].Real - 0.303789), 0.000001);
            Assert.Less(Math.Abs(postProcessedOutput.pMC_R_fx[1].Imaginary - 0.036982), 0.000001);
        }
        ///// <summary>
        ///// Test to validate that calling dMC results in not a NaN
        ///// </summary>
        //[Test]
        //public void validate_dMC_CAW_dROfRhodMua_produces_not_NaN_results()
        //{
        //    var postProcessor = new PhotonDatabasePostProcessor(
        //        VirtualBoundaryType.pMCDiffuseReflectance,
        //        new List<IDetectorInput>()
        //        {
        //            new dMCdROfRhodMuaDetectorInput()
        //            {
        //                Rho = new DoubleRange(0.0, 10, 101),
        //                // set perturbed ops to reference ops
        //                PerturbedOps = new List<OpticalProperties>()
        //                {
        //                    _referenceInputOneLayerTissue.TissueInput.Regions[0].RegionOP,
        //                    new OpticalProperties(
        //                        _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.Mua,
        //                        _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.Musp,
        //                        _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.G,
        //                        _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.N),
        //                    _referenceInputOneLayerTissue.TissueInput.Regions[2].RegionOP
        //                },
        //                PerturbedRegionsIndices = new List<int>() {1}
        //            },
        //            new dMCdROfRhodMusDetectorInput()
        //            {
        //                Rho = new DoubleRange(0.0, 10, 101),
        //                // set perturbed ops to reference ops
        //                PerturbedOps = new List<OpticalProperties>()
        //                {
        //                    _referenceInputOneLayerTissue.TissueInput.Regions[0].RegionOP,
        //                    new OpticalProperties(
        //                        _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.Mua,
        //                        _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.Musp,
        //                        _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.G,
        //                        _referenceInputOneLayerTissue.TissueInput.Regions[1].RegionOP.N),
        //                    _referenceInputOneLayerTissue.TissueInput.Regions[2].RegionOP
        //                },
        //                PerturbedRegionsIndices = new List<int>() {1}
        //            }
        //        },
        //        _databaseOneLayerTissue,
        //        _referenceInputOneLayerTissue);
        //    var postProcessedOutput = postProcessor.Run();
        //    // validation value obtained from linux run using above input and seeded the same
        //    Assert.AreNotEqual(Math.Abs(postProcessedOutput.dMCdMua_R_r[0]), double.NaN);
        //    Assert.AreNotEqual(Math.Abs(postProcessedOutput.dMCdMus_R_r[0]), double.NaN);
        //}
    }
}

