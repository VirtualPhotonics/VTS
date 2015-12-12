using System.Linq;
using Vts.MonteCarlo.DataStructuresValidation;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This verifies ReflectedDynamicMTOfXAndYAndSubregionHistDetectorInput
    /// </summary>
    public class ReflectedDynamicMTOfXAndYAndSubregionHistDetectorInputValidation
    {
        /// <summary>
        /// Method to validate blood volume fraction input agrees with number of tissue subregions
        /// </summary>
        /// <param name="input">detector input in SimulationInput</param>
        /// <returns></returns>
        public static ValidationResult ValidateInput(IDetectorInput input, int tissueRegionCount)
        {
            // test if blood volume fraction list length agrees with number of tissue regions
            if (((dynamic)input).BloodVolumeFraction.Count != tissueRegionCount)
            {
                return new ValidationResult(
                    false,
                    "ReflectedDynamicMTOfXAndYAndSubregionHistDetectorInput: blood volume fraction list length needs to match number of tissue subregions",
                    "Modify list of blood volume fraction to agree with tissue regions");
            }
            return new ValidationResult(
                true,
                "ReflectedDynamicMTOfXAndYAndSubregionHistDetectorInput: blood volume fraction list agrees with number of tissue regions");
        }
    }
}
