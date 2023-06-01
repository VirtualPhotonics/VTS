using System.IO.Compression;
using Vts.MonteCarlo.DataStructuresValidation;
using Vts.MonteCarlo.Sources;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This verifies the structure of a FluorescentEmissionAOfRhoAndZSourceInput
    /// </summary>
    public class FluorescenceEmissionAOfRhoAndZSourceInputValidation
    {
        /// <summary>
        /// Method to warn that if Uniform sampling is specified, only one
        /// fluorescing voxel can be simulated
        /// </summary>
        /// <param name="input">source input in SimulationInput</param>
        /// <returns>An instance of the ValidationResult class</returns>
        public static ValidationResult ValidateInput(ISourceInput input)
        {
            if (((dynamic)input).SamplingMethod == SourcePositionSamplingType.Uniform)
            {
                return new ValidationResult(true, 
                    "A Uniform Sampling Type requires that only one voxel is fluorescing");
            }

            return new ValidationResult(true,"");
        }
       
    }
}
