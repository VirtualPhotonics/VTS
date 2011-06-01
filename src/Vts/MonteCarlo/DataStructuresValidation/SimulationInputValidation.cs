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
        private static SimulationInput _input;
        public static ValidationResult ValidateInput(SimulationInput input)
        {
            _input = input;
            var result = new ValidationResult();
            result = ValidateN(_input.N);
            if (result.IsValid == false) return result;
            result = ValidateTissueInput(input.TissueInput);
            if (result.IsValid == false) return result;
            //ValidateSourceInput(input.SourceInput);
            //ValidateDetectorInput(input.DetectorInputs);
            return result;
        }

        private static ValidationResult ValidateN(long N)
        {
            var result = new ValidationResult();
            if (N < 10)
            {
                result.IsValid = false;
                result.ErrorMessage = "Number of photons value is in error";
                result.Remarks = "Number of photons has to be greater than 9";
            }
            return result;
        }

        private static ValidationResult ValidateTissueInput(ITissueInput tissueInput)
        {
            var result = new ValidationResult();
            if (tissueInput is MultiLayerTissueInput)
            {
                result = MultiLayerTissueInputValidation.ValidateInput(tissueInput.Regions);
            }
            return result;
        }
    }
}
