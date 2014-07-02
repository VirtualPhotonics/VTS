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
            return new List<PostProcessorInput>()
            {
                PostProcessorROfRho(),
                pMCROfRhoROfRhoAndTime()
            };
        }


        #region PostProcessor R(rho)
        /// <summary>
        /// Perturbation MC R(rho) 
        /// </summary>
        public static PostProcessorInput PostProcessorROfRho()
        {
            return new PostProcessorInput(
                new List<IDetectorInput>()
                {
                    new ROfRhoDetectorInput
                    {
                        Rho = new DoubleRange(0.0, 10, 101)
                    }
                },
                true,
                "one_layer_ROfRho_DAW",
                "one_layer_ROfRho_DAW",
                "PostProcessor_ROfRho"
            );
        }
        #endregion

        #region pMC R(rho) and R(rho,time)
        /// <summary>
        /// Perturbation MC R(rho).  This assumes database being post-processed is for
        /// tissue system with one layer.
        /// </summary>
        public static PostProcessorInput pMCROfRhoROfRhoAndTime()
        {
            return new PostProcessorInput(
                //VirtualBoundaryType.pMCDiffuseReflectance,
                new List<IDetectorInput>()
                {
                    //new pMCROfRhoDetectorInput(
                    //    new DoubleRange(0.0, 10, 101),
                    //    // set perturbed ops to reference ops
                    //    new List<OpticalProperties>() { 
                    //        new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                    //        new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                    //        new OpticalProperties(0.0, 1e-10, 1.0, 1.0)
                    //    },
                    //    new List<int>() { 1 }),
                    //new pMCROfRhoDetectorInput(
                    //    new DoubleRange(0.0, 10, 101),
                    //    new List<OpticalProperties>() { 
                    //            new OpticalProperties(0.0, 1e-10, 0.0, 1.0),
                    //            new OpticalProperties(0.01, 1.5, 0.8, 1.4),
                    //            new OpticalProperties(0.0, 1e-10, 0.0, 1.0)},
                    //    new List<int>() { 1 },
                    //    "pMCROfRho_mus1p5"),
                    //new pMCROfRhoDetectorInput(
                    //    new DoubleRange(0.0, 10, 101),
                    //    new List<OpticalProperties>() { 
                    //            new OpticalProperties(0.0, 1e-10, 0.0, 1.0),
                    //            new OpticalProperties(0.01, 0.5, 0.8, 1.4),
                    //            new OpticalProperties(0.0, 1e-10, 0.0, 1.0)},
                    //    new List<int>() { 1 },
                    //    "pMCROfRho_mus0p5"),
                    //new pMCROfRhoAndTimeDetectorInput( 
                    //    new DoubleRange(0.0, 10, 101),
                    //    new DoubleRange(0.0, 10, 101),
                    //    new List<OpticalProperties>() {
                    //        new OpticalProperties(0.0, 1e-10, 0.0, 1.0),
                    //        new OpticalProperties(0.01, 1.5, 0.8, 1.4),
                    //        new OpticalProperties(0.0, 1e-10, 0.0, 1.0)},
                    //    new List<int>() { 1 },
                    //    "pMCROfRhoAndTime_mus1p5"),
                },
                true,
                "pMC_one_layer_ROfRho_DAW",
                "pMC_one_layer_ROfRho_DAW",
                "PostProcessor_pMC_ROfRhoROfRhoAndTime"
            );
        }
        #endregion
    }
}
