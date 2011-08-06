using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.DataStructuresValidation;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This verifies that the DetectorInputs in this class are surface type detectors. 
    /// </summary>
    /// <param name="layers"></param>
    public class SurfaceVirtualBoundaryInputValidation
    {
        public static ValidationResult ValidateInput(IVirtualBoundaryInput vbInput)
        {
            foreach (var detectorInput in vbInput.DetectorInputs)
	        {
                if (!detectorInput.TallyType.IsSurfaceTally())
                {
                    return new ValidationResult(
                        false,
                        "SurfaceVirtualBoundaryInput: detector input is not a surface type",
                        "Make sure IList<IDetectorInput> only contains surface type tallies");
                }
                if ((vbInput.VirtualBoundaryType.IsReflectanceSurfaceVirtualBoundary() &&
                    !detectorInput.TallyType.IsReflectanceTally()) ||
                    (vbInput.VirtualBoundaryType.IsTransmittanceSurfaceVirtualBoundary() &&
                    !detectorInput.TallyType.IsTransmittanceTally()) ||
                    (vbInput.VirtualBoundaryType.IsInternalSurfaceVirtualBoundary() &&
                    !detectorInput.TallyType.IsInternalSurfaceTally()))
                {
                    return new ValidationResult(
                        false,
                        "SurfaceVirtualBoundaryInput: detector input not consistent with virtual boundary type",
                        "Make sure virtual boundary type matches type of detector input");
                }
            }

            return new ValidationResult(
                true,
                "detector inputs must match type of virtual boundary input");
        }
    }
}
