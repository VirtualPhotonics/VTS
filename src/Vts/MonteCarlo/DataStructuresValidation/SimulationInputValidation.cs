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

            tempResult = ValidateSourceInput(input.SourceInput, input.TissueInput);
            if (!tempResult.IsValid)
            {
                return tempResult;
            }
            
            tempResult = ValidateTissueInput(input.TissueInput);
            if (!tempResult.IsValid)
            {
                return tempResult;
            }

            tempResult = ValidateVirtualBoundaryInput(input.VirtualBoundaryInputs);
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

        private static ValidationResult ValidateSourceInput(ISourceInput sourceInput, ITissueInput tissueInput)
        {
            if ((sourceInput.InitialTissueRegionIndex < 0) ||
                (sourceInput.InitialTissueRegionIndex > tissueInput.Regions.Count - 1))
            {
                return new ValidationResult(
                    false,
                    "Source input not valid given tissue definition",
                    "Alter sourceInput.InitialTissueRegionIndex to be consistent with tissue definition");
            }
            else
            {
                return new ValidationResult(
                    true,
                    "Starting photons in region " + sourceInput.InitialTissueRegionIndex);
            }
        }

        private static ValidationResult ValidateTissueInput(ITissueInput tissueInput)
        {
            if (tissueInput is MultiLayerTissueInput)
            {
                return MultiLayerTissueInputValidation.ValidateInput(tissueInput);
            }
            if (tissueInput is SingleEllipsoidTissueInput)
            {
                return SingleEllipsoidTissueInputValidation.ValidateInput(tissueInput);
            }

            return new ValidationResult(
                true,
                "Tissue input must be valid",
                "Validation skipped for tissue input " + tissueInput + ". No matching validation rules were found.");
        }

        private static ValidationResult ValidateVirtualBoundaryInput(IList<IVirtualBoundaryInput> virtualBoundaryInputs)
        {
            bool hasDiffuseReflectanceVB = false;
            bool hasDiffuseTransmittanceVB = false;
            ValidationResult tempResult = null;

            if (virtualBoundaryInputs == null)
            {
                return new ValidationResult(
                    false,
                    "No Virtual Boundaries specified",
                    "Need to specify Virtual Boundaries in order to define detectors");
            }
            foreach (var virtualBoundaryInput in virtualBoundaryInputs)
            {
                switch (virtualBoundaryInput.VirtualBoundaryType)
                {
                    case VirtualBoundaryType.DiffuseReflectance:
                        hasDiffuseReflectanceVB = true;
                        tempResult = SurfaceVirtualBoundaryInputValidation.ValidateInput(virtualBoundaryInput);
                        break;
                    case VirtualBoundaryType.DiffuseTransmittance:
                        hasDiffuseTransmittanceVB = true;
                        tempResult = SurfaceVirtualBoundaryInputValidation.ValidateInput(virtualBoundaryInput);
                        break;
                    case VirtualBoundaryType.GenericVolumeBoundary:
                        tempResult = GenericVolumeVirtualBoundaryInputValidation.ValidateInput(virtualBoundaryInput);
                        break;
                    case VirtualBoundaryType.pMCDiffuseReflectance:
                        hasDiffuseReflectanceVB = true;
                        tempResult = pMCSurfaceVirtualBoundaryInputValidation.ValidateInput(virtualBoundaryInput);
                        break;
                    case VirtualBoundaryType.SpecularReflectance:
                        tempResult = SurfaceVirtualBoundaryInputValidation.ValidateInput(virtualBoundaryInput);
                        break;
                    case VirtualBoundaryType.SurfaceRadiance:
                        tempResult = SurfaceVirtualBoundaryInputValidation.ValidateInput(virtualBoundaryInput);
                        break;
                    default:
                        return new ValidationResult(
                            false,
                            "VirtualBoundaryInput VirtualBoundaryType in error",
                            "Verify VirtualBoundaryType is valid");

                }
                if (!tempResult.IsValid)
                {
                    return tempResult;
                }
            }
            if (!hasDiffuseReflectanceVB || !hasDiffuseTransmittanceVB)
            {
                return new ValidationResult(
                    false,
                    "DiffuseReflectance and DiffuseTransmittance VirtualBoundaryInput are required (can have null list of detectors)",
                    "Add SurfaceVirtualBoundary for both VirtualBoundaryType.DiffuseReflectance and .DiffuseTransmittance");
            }
            return new ValidationResult(
                true,
                "Virtual Boundary input must be valid",
                "");
        }
    }
}
