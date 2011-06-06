using System;
using System.Collections.Generic;
using Vts.MonteCarlo;
using Vts.MonteCarlo.DataStructuresValidation;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This sanity checks SimulationInput
    /// </summary>
    public class SimulationInputValidation
    {
        public static ValidationResult ValidateInput(SimulationInput input)
        {
            ValidationResult tempResult;

            tempResult = ValidateN(input.N);
            if (!tempResult.IsValid)
            {
                return tempResult;
            }

            tempResult = ValidateTissueInput(input.TissueInput);
            if (!tempResult.IsValid)
            {
                return tempResult;
            }

            return new ValidationResult(
                true,
                "Simulation input must be valid");
        }

        private static ValidationResult ValidateN(long N)
        {
            return new ValidationResult(
                N >= 10,
                "Number of photons must be greater than 9",
                "This is an implementation detail of the MC simulation");
        }

        private static ValidationResult ValidateTissueInput(ITissueInput tissueInput)
        {
            if (tissueInput is MultiLayerTissueInput)
            {
                return MultiLayerTissueInputValidation.ValidateInput(tissueInput.Regions);
            }  

            return new ValidationResult(
                true,
                "Tissue input must be valid",
                "Validation skipped for tissue input " + tissueInput + ". No matching validation rules were found.");
        }
    }
}
