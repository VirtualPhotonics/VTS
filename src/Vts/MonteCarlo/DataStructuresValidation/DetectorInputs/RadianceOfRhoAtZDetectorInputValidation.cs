using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Tissues;
using ValidationResult = Vts.MonteCarlo.DataStructuresValidation.ValidationResult;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This verifies RadianceOfRhoAtZDetectorInput
    /// </summary>
    public static class RadianceOfRhoAtZDetectorInputValidation
    {
        /// <summary>
        /// Method to validate specified ZDepth is at tissue layer definition
        /// </summary>
        /// <param name="input">detector input in SimulationInput</param>
        /// <param name="tissueLayers">tissue layer regions</param>
        /// <returns>An instance of the ValidationResult class</returns>
        public static ValidationResult ValidateInput(IDetectorInput input, IList<LayerTissueRegion> tissueLayers)
        {
            // test if ZDepth is equal to tissue layer definition
            if (!tissueLayers.Any(l => Math.Abs(l.ZRange.Stop - ((RadianceOfRhoAtZDetectorInput)input).ZDepth) < 1E-10))
            {
                return new ValidationResult(
                    false,
                    "RadianceOfRhoAtZDetectorInput: detector ZDepth needs to be equal to a tissue layer interface",
                    "Modify ZDepth to agree with tissue layer definitions or add tissue layer at ZDepth");
            }
            // test if ZDepth is at top or bottom of tissue definition
            if (Math.Abs(tissueLayers[0].ZRange.Stop - ((RadianceOfRhoAtZDetectorInput)input).ZDepth) < 1E-10 ||
                Math.Abs(tissueLayers[^1].ZRange.Start - ((RadianceOfRhoAtZDetectorInput)input).ZDepth) < 1E-10)
            {
                return new ValidationResult(
                    false,
                    "RadianceOfRhoAtZDetectorInput: detector ZDepth cannot be specified at top or bottom of tissue definition",
                    "Modify ZDepth to agree with tissue layer but not at top or bottom");

            }
            return new ValidationResult(
                true,
                "RadianceOfRhoAtZDetectorInput: ZDepth agrees with tissue layer specifications");
        }
    }
}