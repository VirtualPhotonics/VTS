using Vts.MonteCarlo.DataStructuresValidation;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This verifies any Recessed DetectorInput
    /// </summary>
    public class RecessedDetectorInputValidation
    {
        /// <summary>
        /// Method to validate recessed ZPlane specification is negative
        /// </summary>
        /// <param name="input">detector input in SimulationInput</param>
        /// <returns>ValidationResult class</returns>
        public static ValidationResult ValidateInput(IDetectorInput input)
        {
            // test if ZPlane is negative or zero
            if (((dynamic)input).ZPlane > 0.0)
            {
                return new ValidationResult(
                    false,
                    "RecessedDetectorInput: recessed ZPlane must be on or above tissue",
                    "Modify ZPlane to be non-positive");
            }
            return new ValidationResult(
                true,
                "RecessedDetectorInput: ZPlane is zero or negative");
        }
    }
}
