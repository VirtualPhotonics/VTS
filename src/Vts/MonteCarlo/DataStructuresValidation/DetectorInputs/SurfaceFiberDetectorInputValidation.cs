using Vts.MonteCarlo.DataStructuresValidation;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This verifies SurfaceFiberDetectorInput is correct
    /// </summary>
    public static class SurfaceFiberDetectorInputValidation
    {
        /// <summary>
        /// Method to validate detector fiber is defined to be on surface of tissue
        /// </summary>
        /// <param name="input">detector input in SimulationInput</param>
        /// <returns>An instance of the ValidationResult class</returns>
        public static ValidationResult ValidateInput(IDetectorInput input)
        {
            // test if detector center is not on surface of tissue
            if (((SurfaceFiberDetectorInput)input).Center.Z != 0.0)
            {
                return new ValidationResult(
                    false,
                    "SurfaceFiberDetectorInput: detector needs to be defined to be on surface of tissue",
                    "Modify detector Center Z value to be 0.0");
            }
            return new ValidationResult(
                true,
                "SurfaceDetectorInput: detector needs to be defined to be on surface of tissue");
        }
    }
}
