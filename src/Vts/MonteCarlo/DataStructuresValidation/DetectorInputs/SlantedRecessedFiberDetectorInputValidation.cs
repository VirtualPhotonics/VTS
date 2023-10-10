using System;
using Vts.MonteCarlo.DataStructuresValidation;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This verifies SlantedRecessedFiberDetectorInput is correct
    /// </summary>
    public static class SlantedRecessedFiberDetectorInputValidation
    {
        /// <summary>
        /// Method to validate slanted recessed fiber detector inputs
        /// </summary>
        /// <param name="input">detector input in SimulationInput</param>
        /// <returns>An instance of the ValidationResult class</returns>
        public static ValidationResult ValidateInput(IDetectorInput input)
        {
            // test if contact position of the detector fiber is defined to be on 
            // surface of tissue or above
            if (((SlantedRecessedFiberDetectorInput)input).ZPlane > 0.0)
            {
                return new ValidationResult(
                    false,
                    "SlantedRecessedFiberDetectorInput: contact position of the detector needs to be defined to be " +
                    "on surface of tissue or above", "Modify ContactPosition Z value to be 0.0 or negative");
            }

            // test if radius of the detector fiber is positive
            if (((SlantedRecessedFiberDetectorInput)input).Radius <= 0.0)
            {
                return new ValidationResult(
                    false,
                    "SlantedRecessedFiberDetectorInput: radius cannot be zero or negative",
                    "Modify radius to be positive");
            }

            return ((SlantedRecessedFiberDetectorInput)input).Angle switch
            {
                // test if angle of the detector fiber is not negative
                < 0.0 => new ValidationResult(false, "SlantedRecessedFiberDetectorInput: angle cannot be negative",
                    "Modify angle to be zero or positive"),
                // test if angle of the detector fiber is not negative
                >= Math.PI / 2.0 => new ValidationResult(false,
                    "SlantedRecessedFiberDetectorInput: angle cannot be 90 degrees or more",
                    "Modify angle to be below 90 degrees"),
                _ => new ValidationResult(true, "SlantedRecessedDetectorInput: inputs are properly defined")
            };
        }
    }
}
