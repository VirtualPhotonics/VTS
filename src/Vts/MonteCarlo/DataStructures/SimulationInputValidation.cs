using System;
using System.Collections.Generic;
using Vts.MonteCarlo;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This sanity checks SimulationInput
    /// </summary>
    public class SimulationInputValidation
    {
        private static SimulationInput _input;
        public static void ValidateInput(SimulationInput input)
        {
            _input = input;
            if (_input.N < 10)
            {
                throw new ArgumentException(
                    "Number of photons has to be greater than 9");
            }
            ValidateTissueInput(input.TissueInput);
            //ValidateSourceInput(input.SourceInput);
            //ValidateDetectorInput(input.DetectorInputs);
        }

        private static void ValidateTissueInput(ITissueInput tissueInput)
        {
            if (tissueInput is MultiLayerTissueInput)
            {
                MultiLayerTissueInput.ValidateInput(tissueInput.Regions);
            }
        }
    }
}
