using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Implements various commonly used PostProcessorInput classes for various tissue types.
    /// </summary>
    public class PostProcessorInputProvider : PostProcessorInput
    {
        /// <summary>
        /// Method that provides instances of all inputs in this class.
        /// </summary>
        /// <returns>a list of the PostProcessorInputs generated</returns>
        public static IList<PostProcessorInput> GenerateAllPostProcessorInputs()
        {
            // additions to this list need to be added to MCPP Program tests for clean up
            return new List<PostProcessorInput>
            {
                PostProcessorROfRhoTOfRho(), // no perturbation, just gen of regular detectors
                pMCROfRhoAndROfRhoAndTime(), // don't change this it is part of documentation
                pMCROfRhoROfXAndYVariants(),
                pMCROfFxROfFxAndTime()
            };
        }


        #region PostProcessor R(rho) and T(rho)
        /// <summary>
        /// Perturbation MC R(rho) 
        /// </summary>
        /// <returns>An instance of the PostProcessorInput class</returns>
        public static PostProcessorInput PostProcessorROfRhoTOfRho()
        {
            return new PostProcessorInput(
                new List<IDetectorInput>
                {
                    new ROfRhoDetectorInput
                    {
                        Rho = new DoubleRange(0.0, 10, 101)
                    },           
                    new TOfRhoDetectorInput
                    {
                        Rho = new DoubleRange(0.0, 10, 101),
                        FinalTissueRegionIndex = 3
                    }
                },
                "two_layer_ROfRho_TOfRho_with_databases",
                "two_layer_ROfRho_TOfRho_with_databases",
                "PostProcessor_ROfRhoTOfRho"
            );
        }
        #endregion

        #region pMC R(rho) and R(rho,time) part of website documentation so don't modify
        /// <summary>
        /// Perturbation MC R(rho), R(rho) recessed, R(rho,time).  
        /// This assumes database being post-processed is for tissue system with one layer.
        /// </summary>
        /// <returns>An instance of the PostProcessorInput class</returns>
        public static PostProcessorInput pMCROfRhoAndROfRhoAndTime()
        {
            return new PostProcessorInput(
                new List<IDetectorInput>
                {
                    // add in regular ROfRho and ROfRhoAndTime detectors for comparison
                    new ROfRhoDetectorInput
                    {
                        Rho=new DoubleRange(0.0, 10, 101),
                        TallySecondMoment = true,
                    },
                    new ROfRhoAndTimeDetectorInput
                    {
                        Rho=new DoubleRange(0.0, 10, 101),
                        Time=new DoubleRange(0.0, 10, 101),
                    },
                    new pMCROfRhoDetectorInput
                    {
                        Rho=new DoubleRange(0.0, 10, 101),
                        PerturbedOps =      // set perturbed ops to reference ops
                            new List<OpticalProperties>
                            {
                                new(0.0, 1e-10, 1.0, 1.0),
                                new(0.01, 1.0, 0.8, 1.4),
                                new(0.0, 1e-10, 1.0, 1.0)
                            },
                        PerturbedRegionsIndices = new List<int> { 1 },
                        TallySecondMoment = true,
                        Name="pMCROfRhoReference",
                    },
                    new pMCROfRhoDetectorInput
                    {
                        Rho=new DoubleRange(0.0, 10, 101),
                        PerturbedOps =  // perturb mus' by +50%
                            new List<OpticalProperties>
                            {
                                new(0.0, 1e-10, 0.0, 1.0),
                                new(0.01, 1.5, 0.8, 1.4),
                                new(0.0, 1e-10, 0.0, 1.0)
                            },
                        PerturbedRegionsIndices = new List<int> { 1 },
                        TallySecondMoment = true,
                        Name="pMCROfRho_mus1p5",
                    },
                    new pMCROfRhoDetectorInput
                    {
                        Rho=new DoubleRange(0.0, 10, 101),
                        PerturbedOps = // perturb mus' by -50%
                            new List<OpticalProperties>
                            {
                                new(0.0, 1e-10, 0.0, 1.0),
                                new(0.01, 0.5, 0.8, 1.4),
                                new(0.0, 1e-10, 0.0, 1.0)
                            },
                        PerturbedRegionsIndices = new List<int> { 1 },
                        TallySecondMoment = true,
                        Name="pMCROfRho_mus0p5",
                    },
                    new pMCROfRhoAndTimeDetectorInput
                    {
                        Rho=new DoubleRange(0.0, 10, 101),
                        Time=new DoubleRange(0.0, 10, 101),
                        PerturbedOps =
                            new List<OpticalProperties>
                            {
                                new(0.0, 1e-10, 0.0, 1.0),
                                new(0.01, 1.0, 0.8, 1.4),
                                new(0.0, 1e-10, 0.0, 1.0)
                            },
                        PerturbedRegionsIndices = new List<int> { 1 },
                        Name="pMCROfRhoAndTime_reference"
                    },
                    new pMCROfRhoAndTimeDetectorInput
                    {
                        Rho=new DoubleRange(0.0, 10, 101),
                        Time=new DoubleRange(0.0, 10, 101),
                        PerturbedOps =
                            new List<OpticalProperties>
                            {
                                new(0.0, 1e-10, 0.0, 1.0),
                                new(0.01, 1.5, 0.8, 1.4),
                                new(0.0, 1e-10, 0.0, 1.0)
                            },
                        PerturbedRegionsIndices = new List<int> { 1 },
                        Name="pMCROfRhoAndTime_mus1p5"
                    },
                    new pMCROfRhoAndTimeDetectorInput
                    {
                        Rho=new DoubleRange(0.0, 10, 101),
                        Time=new DoubleRange(0.0, 10, 101),
                        PerturbedOps =
                            new List<OpticalProperties>
                            {
                                new(0.0, 1e-10, 0.0, 1.0),
                                new(0.01, 0.5, 0.8, 1.4),
                                new(0.0, 1e-10, 0.0, 1.0)
                            },
                        PerturbedRegionsIndices = new List<int> { 1 },
                        Name="pMCROfRhoAndTime_mus0p5"
                    },
                 },
                "pMC_one_layer_ROfRho_DAW",
                "pMC_one_layer_ROfRho_DAW",
                "PostProcessor_pMC_ROfRhoROfRhoAndTime"
            );
        }
        #endregion

        #region pMC R(rho) variants and pMC ATotal that include recessed detectors 
        /// <summary>
        /// Perturbation MC R(rho) recessed, R(rho,time) recessed, R(rho,maxdepth) recessed
        /// This assumes database being post-processed is for tissue system with one layer.
        /// </summary>
        /// <returns>An instance of the PostProcessorInput class</returns>
        public static PostProcessorInput pMCROfRhoROfXAndYVariants()
        {
            return new PostProcessorInput(
                new List<IDetectorInput>
                {
                    new pMCROfRhoRecessedDetectorInput
                    {
                        Rho=new DoubleRange(0.0, 10, 101),
                        ZPlane=-1.0,
                        PerturbedOps =  // perturb mus' by +50%
                            new List<OpticalProperties>
                            {
                                new(0.0, 1e-10, 0.0, 1.0),
                                new(0.01, 1.5, 0.8, 1.4),
                                new(0.0, 1e-10, 0.0, 1.0)
                            },
                        PerturbedRegionsIndices = new List<int> { 1 },
                        TallySecondMoment = true,
                        Name="pMCROfRhoRecessed_mus1p5",
                    },
                     new pMCROfRhoAndTimeRecessedDetectorInput
                     {
                        Rho=new DoubleRange(0.0, 10, 101),
                        Time=new DoubleRange(0.0, 10, 101),
                        ZPlane=-1.0,
                        PerturbedOps =
                            new List<OpticalProperties>
                            {
                                new(0.0, 1e-10, 0.0, 1.0),
                                new(0.01, 1.5, 0.8, 1.4),
                                new(0.0, 1e-10, 0.0, 1.0)
                            },
                        PerturbedRegionsIndices = new List<int> { 1 },
                        Name="pMCROfRhoAndTimeRecessed_mus1p5"
                    },
                     new pMCROfXAndYAndTimeAndSubregionDetectorInput
                     {
                         X=new DoubleRange(-10, 10, 101),
                         Y=new DoubleRange(-10, 10, 101),
                         Time=new DoubleRange(0.0, 1, 101),
                         PerturbedOps =  // perturb mus' by +50%
                             new List<OpticalProperties>
                             {
                                 new(0.0, 1e-10, 0.0, 1.0),
                                 new(0.01, 1.5, 0.8, 1.4),
                                 new(0.0, 1e-10, 0.0, 1.0)
                             },
                         PerturbedRegionsIndices = new List<int> { 1 },
                         TallySecondMoment = true,
                         Name="pMCROfXAndYAndTimeAndSubregion_mus1p5",
                     },
                     new pMCROfXAndYAndTimeAndSubregionRecessedDetectorInput
                     {
                         X=new DoubleRange(-10, 10, 101),
                         Y=new DoubleRange(-10, 10, 101),
                         Time=new DoubleRange(0.0, 1, 101),
                         ZPlane=-1.0,
                         PerturbedOps =
                             new List<OpticalProperties>
                             {
                                 new(0.0, 1e-10, 0.0, 1.0),
                                 new(0.01, 1.5, 0.8, 1.4),
                                 new(0.0, 1e-10, 0.0, 1.0)
                             },
                         PerturbedRegionsIndices = new List<int> { 1 },
                         Name="pMCROfXAndYAndTimeAndSubregionRecessed_mus1p5"
                     },
                     new pMCATotalDetectorInput
                     {
                         PerturbedOps =
                             new List<OpticalProperties>
                             {
                                 new(0.0, 1e-10, 0.0, 1.0),
                                 new(0.01, 1.5, 0.8, 1.4),
                                 new(0.0, 1e-10, 0.0, 1.0)
                             },
                         PerturbedRegionsIndices = new List<int> { 1 },
                         Name="pMCATotal_mus1p5"
                     },
                },
                "pMC_one_layer_ROfRho_DAW",
                "pMC_one_layer_ROfRho_DAW",
                "PostProcessor_pMC_ROfRhoROfXAndYVariants"
            );
        }
        #endregion

        #region pMC and dMC R(fx) and R(fx,time)
        /// <summary>
        /// Perturbation MC R(fx) and R(fx,time). This assumes database being post-processed is for
        /// tissue system with one layer.
        /// </summary>
        /// <returns>An instance of the PostProcessorInput class</returns>
        public static PostProcessorInput pMCROfFxROfFxAndTime()
        {
            return new PostProcessorInput(
                //VirtualBoundaryType.pMCDiffuseReflectance,
                new List<IDetectorInput>
                {
                    new pMCROfFxDetectorInput
                    {
                        Fx=new DoubleRange(0.0, 0.5, 11),
                        PerturbedOps =      // set perturbed ops to reference ops
                            new List<OpticalProperties>
                            { 
                                new(0.0, 1e-10, 1.0, 1.0),
                                new(0.01, 1.0, 0.8, 1.4),
                                new(0.0, 1e-10, 1.0, 1.0)
                            },
                        PerturbedRegionsIndices = new List<int> { 1 },
                        TallySecondMoment = true,
                        Name="pMCROfFxReference",
                    },
                    new pMCROfFxDetectorInput
                    {                      
                        Fx=new DoubleRange(0.0, 0.5, 11),
                        PerturbedOps =  // perturb mus' by +50%
                            new List<OpticalProperties>
                            { 
                                new(0.0, 1e-10, 0.0, 1.0),
                                new(0.01, 1.5, 0.8, 1.4),
                                new(0.0, 1e-10, 0.0, 1.0)
                            },
                        PerturbedRegionsIndices = new List<int> { 1 },
                        TallySecondMoment = true,
                        Name="pMCROfFx_mus1p5",
                    },
                    new pMCROfFxDetectorInput
                    {                        
                        Fx=new DoubleRange(0.0, 0.5, 11),
                        PerturbedOps = // perturb mus' by -50%
                            new List<OpticalProperties>
                            { 
                                new(0.0, 1e-10, 0.0, 1.0),
                                new(0.01, 0.5, 0.8, 1.4),
                                new(0.0, 1e-10, 0.0, 1.0)
                            },
                        PerturbedRegionsIndices = new List<int> { 1 },
                        TallySecondMoment = true,
                        Name="pMCROfFx_mus0p5",
                    },
                    new pMCROfFxAndTimeDetectorInput
                    {                        
                        Fx=new DoubleRange(0.0, 0.5, 11),
                        Time=new DoubleRange(0.0, 10, 101),
                        PerturbedOps = 
                            new List<OpticalProperties>
                            {
                                new(0.0, 1e-10, 0.0, 1.0),
                                new(0.01, 1.5, 0.8, 1.4),
                                new(0.0, 1e-10, 0.0, 1.0)
                            },
                        PerturbedRegionsIndices = new List<int> { 1 },
                        Name="pMCROfFxAndTime_mus1p5"
                    },
                    new dMCdROfFxdMuaDetectorInput
                    {
                        Fx=new DoubleRange(0.0, 0.5, 11),
                        PerturbedOps = // perturb mus' by -50%
                            new List<OpticalProperties>
                            {
                                new(0.0, 1e-10, 0.0, 1.0),
                                new(0.01, 0.5, 0.8, 1.4),
                                new(0.0, 1e-10, 0.0, 1.0)
                            },
                        PerturbedRegionsIndices = new List<int> { 1 },
                        TallySecondMoment = true,
                        Name="dMCdROfFxdMua_mus0p5",
                    },
                    new dMCdROfFxdMusDetectorInput
                    {
                        Fx=new DoubleRange(0.0, 0.5, 11),
                        PerturbedOps = // perturb mus' by -50%
                            new List<OpticalProperties>
                            {
                                new(0.0, 1e-10, 0.0, 1.0),
                                new(0.01, 0.5, 0.8, 1.4),
                                new(0.0, 1e-10, 0.0, 1.0)
                            },
                        PerturbedRegionsIndices = new List<int> { 1 },
                        TallySecondMoment = true,
                        Name="dMCdROfFxdMus_mus0p5",
                    }
                },
                "pMC_one_layer_ROfRho_DAW",
                "pMC_one_layer_ROfRho_DAW",
                "PostProcessor_pMC_ROfFxROfFxAndTime"
            );
        }
        #endregion
    }
}
