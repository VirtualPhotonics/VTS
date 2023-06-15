using Vts.MonteCarlo.DataStructuresValidation;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This verifies TransmittedDynamicMTOfRhoAndSubregionHistDetectorInput
    /// </summary>
    public static class TransmittedDynamicMTOfRhoAndSubregionHistDetectorInputValidation
    {
        /// <summary>
        /// Method to validate blood volume fraction input agrees with number of tissue subregions
        /// </summary>
        /// <param name="input">detector input in SimulationInput</param>
        /// <param name="tissueRegionCount">number of tissue regions</param>
        /// <returns>An instance of the ValidationResult class</returns>
        public static ValidationResult ValidateInput(IDetectorInput input, int tissueRegionCount)
        {
            // test if blood volume fraction list length agrees with number of tissue regions
            if (((TransmittedDynamicMTOfRhoAndSubregionHistDetectorInput)input).BloodVolumeFraction.Count != tissueRegionCount)
            {
                return new ValidationResult(
                    false,
                    "TransmittedDynamicMTOfRhoAndSubregionHistDetectorInput: blood volume fraction list length needs to match number of tissue subregions",
                    "Modify list of blood volume fraction to agree with tissue regions");
            }
            return new ValidationResult(
                true,
                "TransmittedDynamicMTOfRhoAndSubregionHistDetectorInput: blood volume fraction list agrees with number of tissue regions");
        }
    }
}
