using Vts.MonteCarlo.DataStructuresValidation;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This verifies dMCROfRhoDMuaDetectorInput
    /// </summary>
    public static class dMCdROfRhodMuaDetectorInputValidation
    {
        /// <summary>
        /// Method to validate that only one perturbed region specified
        /// </summary>
        /// <param name="input">detector input in SimulationInput</param>
        /// <returns>An instance of the ValidationResult class</returns>
        public static ValidationResult ValidateInput(IDetectorInput input)
        {
            // test if perturbed region indices has only one index
            if (((dMCdROfRhodMuaDetectorInput)input).PerturbedRegionsIndices.Count > 1)
            {
                return new ValidationResult(
                    false,
                    "dMCdROfRhodMuaDetectorInput: current capability allows only 1 perturbed region",
                    "Modify list of perturbedRegionIndices to contain only 1 index");
            }
            return new ValidationResult(
                true,
                "dMCdROfRhodMuaDetectorInput: perturbedRegionIndices only has 1 index");
        }
    }
}
