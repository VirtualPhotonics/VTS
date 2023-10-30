using System;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.DataStructuresValidation;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This sanity checks SimulationInput
    /// </summary>
    public class SimulationInputValidation
    {
        /// <summary>
        /// Master of call validation methods. Calls methods to validate source,
        /// tissue and detector definitions.
        /// </summary>
        /// <param name="input">SimulationInput to be validated</param>
        /// <returns>An instance of ValidationResult with IsValid bool set and message about error if false</returns>
        public static ValidationResult ValidateInput(SimulationInput input)
        {
            var validations = new Func<SimulationInput, ValidationResult>[]
                {
                    si => ValidateN(si.N),
                    si => ValidateSourceInput(si.SourceInput, si.TissueInput),
                    si => ValidateTissueInput(si.TissueInput),
                    si => ValidateDetectorInput(si),
                    si => ValidateCombinedInputParameters(si),
                    si => ValidateCurrentIncapabilities(si)
                };

            foreach (var validation in validations)
            {
                var tempResult = validation(input);
                if (!tempResult.IsValid)
                {
                    return tempResult;
                }
            }
            
            return new ValidationResult( true, "Simulation input is valid");

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
            if (sourceInput.InitialTissueRegionIndex < 0 ||
                sourceInput.InitialTissueRegionIndex > tissueInput.Regions.Length - 1)
            {
                return new ValidationResult(
                    false,
                    "Source input not valid given tissue definition",
                    "Alter sourceInput.InitialTissueRegionIndex to be consistent with tissue definition");
            }
            if (sourceInput is FluorescenceEmissionAOfRhoAndZSourceInput)
            {
                return FluorescenceEmissionAOfRhoAndZSourceInputValidation.ValidateInput(sourceInput);
            }
            if (sourceInput is FluorescenceEmissionAOfXAndYAndZSourceInput)
            {
                return FluorescenceEmissionAOfXAndYAndZSourceInputValidation.ValidateInput(sourceInput);
            }
            return new ValidationResult(
                true,
                "Starting photons in region " + sourceInput.InitialTissueRegionIndex);

        }

        private static ValidationResult ValidateTissueInput(ITissueInput tissueInput)
        {
            // for all types of tissues, check that OPs are non-negative (g could be neg)
            if (tissueInput.Regions.Any(r => r.RegionOP.Mua < 0.0) ||
                tissueInput.Regions.Any(r => r.RegionOP.Musp < 0.0) ||
                tissueInput.Regions.Any(r => r.RegionOP.N < 0.0))
            {
                return new ValidationResult(
                    false,
                     "Tissue optical properties mua, mus', n need to be non-negative",
                     "Please check optical properties");
            }

            if (tissueInput is MultiLayerTissueInput)
            {
                return MultiLayerTissueInputValidation.ValidateInput(tissueInput);
            }
            if (tissueInput is SingleEllipsoidTissueInput)
            {
                return SingleEllipsoidTissueInputValidation.ValidateInput(tissueInput);
            }
            if (tissueInput is SingleVoxelTissueInput)
            {
                return SingleVoxelTissueInputValidation.ValidateInput(tissueInput);
            }
            if (tissueInput is MultiConcentricInfiniteCylinderTissueInput)
            {
                return MultiConcentricInfiniteCylinderTissueInputValidation.ValidateInput(tissueInput);
            }
            if (tissueInput is BoundingCylinderTissueInput)
            {
                return BoundingCylinderTissueInputValidation.ValidateInput(tissueInput);
            }

            return new ValidationResult(
                true,
                "Tissue input must be valid",
                "Validation skipped for tissue input " + tissueInput +
                ". No matching validation rules were found.");
        }
        private static ValidationResult ValidateDetectorInput(SimulationInput si)
        {
            if (si.Options.Databases == null || !si.Options.Databases.Any() && !si.DetectorInputs.Any())
            {
                return new ValidationResult(
                    false,
                    "No detector inputs specified and no database to be written",
                    "Make sure list of DetectorInputs is not empty or null if no databases are to be written");
            }
            // black list of unimplemented detectors
            foreach (var detectorInput in si.DetectorInputs)
            {
                if (detectorInput.TallyDetails.IsNotImplementedYet)
                {
                    return new ValidationResult(
                        false,
                        "DetectorInput not implemented yet:" + detectorInput.ToString(),
                        "Please omit " + detectorInput.ToString() + " from DetectorInput list");
                }
            }
            // make sure all detectors have unique Names
            var allDetectorInputNames = si.DetectorInputs.Select(d => d.Name).ToList();
            if (allDetectorInputNames.Count != allDetectorInputNames.Distinct().Count())
            {
                return new ValidationResult(
                        false,
                        "Duplicate Name used for detector",
                        "Please check for duplicate names and make sure all Names are unique");
            }
            return new ValidationResult(
                true,
                "DetectorInput must be valid",
                "");
        }

        /// <summary>
        /// This method checks the input against combined combinations of options
        /// and source, tissue, detector definitions.   
        /// </summary>
        /// <param name="input">input to be validated</param>
        /// <returns>An instance of ValidationResult with IsValid set and error message if false</returns>
        private static ValidationResult ValidateCombinedInputParameters(SimulationInput input)
        {
            // check that absorption weighting type set to analog and RR weight threshold != 0.0
            if (input.Options.AbsorptionWeightingType == AbsorptionWeightingType.Analog &&
                input.Options.RussianRouletteWeightThreshold != 0.0)
            {
                return new ValidationResult(
                    false,
                    "Russian Roulette cannot be employed with Analog absorption weighting is specified",
                    "With Analog absorption weighting, set Russian Roulette weight threshold = 0.0");
            }
            // check that if single ellipsoid tissue specified and (r,z) detector specified,
            // that (1) ellipsoid is centered at x=0, y=0, (2) ellipsoid is cylindrically symmetric (dx=dy)
            var tissueWithEllipsoid = input.TissueInput as SingleEllipsoidTissueInput;
            if (tissueWithEllipsoid != null)
            {
                var ellipsoid = (EllipsoidTissueRegion)tissueWithEllipsoid.EllipsoidRegion;
                foreach (var detectorInput in input.DetectorInputs)
                {
                    switch (detectorInput.TallyDetails.IsCylindricalTally)
                    {
                        case true when
                            ellipsoid.Center.X != 0.0 && ellipsoid.Center.Y != 0.0:
                            return new ValidationResult(
                                false,
                                "Ellipsoid must be centered at (x,y)=(0,0) for cylindrical tallies",
                                "Change ellipsoid center to (0,0) or specify non-cylindrical type tally");
                        case true when ellipsoid.Dx != ellipsoid.Dy:
                            return new ValidationResult(
                                false,
                                "Ellipsoid must have Dx=Dy for cylindrical tallies",
                                "Change ellipsoid.Dx to be = to Dy or specify non-cylindrical type tally");
                    }

                    if (detectorInput.TallyType == TallyType.ROfFx)
                    {
                        return new ValidationResult(
                            false,
                            "R(fx) tallies assume a homogeneous or layered tissue geometry",
                            "Change tissue type to be homogeneous or layered"); 
                    }
                }
            }
            // check that if single voxel or single infinite cylinder tissue specified,
            // cannot specify (r,z) detector 
            if (input.TissueInput is SingleVoxelTissueInput || 
                input.TissueInput is SingleInfiniteCylinderTissueInput)
            {
                foreach (var detectorInput in input.DetectorInputs)
                {
                    if (detectorInput.TallyDetails.IsCylindricalTally)
                    {
                        return new ValidationResult(
                            false,
                            "Cannot use Single Voxel Tissue for cylindrical tallies",
                            "Change detector inputs to specify non-cylindrical type tallies");
                    }
                    if (detectorInput.TallyType == TallyType.ROfFx)
                    {
                        return new ValidationResult(
                            false,
                            "R(fx) tallies assume a homogeneous or layered tissue geometry",
                            "Change tissue type to be homogeneous or layered");
                    }
                }
            }
            // check that if bounding volume tissue specified, the ATotalBoundingVolumeTissueInput detector needs
            // to be specified
            if (input.TissueInput is BoundingCylinderTissueInput)
            {
                if (input.DetectorInputs.All(d => d.TallyType != TallyType.ATotalBoundingVolume))
                {
                    return new ValidationResult(
                        false,
                        "BoundingCylinderTissueInput needs associated detector ATotalBoundingVolume to be defined",
                        "Add ATotalBoundingVolumeDetectorInput to detector inputs");
                }
            }

            var source = input.SourceInput as DirectionalPointSourceInput;
            if (source != null)
            {
                if (source.Direction != new Direction(0,0,1))
                {
                    foreach (var detectorInput in input.DetectorInputs)
                    {
                        if (detectorInput.TallyDetails.IsCylindricalTally)
                        {
                            return new ValidationResult(
                                false,
                                "If source is angled, cannot define cylindrically symmetric detectors",
                                "Change detector to Cartesian equivalent or define source to be normal"); 
                        }
                    }
                }
            }
            foreach (var detectorInput in input.DetectorInputs)
            {
                if (detectorInput.TallyDetails.IsTransmittanceTally && input.TissueInput is MultiLayerTissueInput)
                {
                    if (((dynamic)detectorInput).FinalTissueRegionIndex == 0)
                    {
                            return new ValidationResult(
                                false,
                                "Transmittance detectors with MultiLayerTissues cannot detect in tissue region 0",
                                "Change FinalTissueRegionIndex to be index of air below tissue (index >= 2)");
                    }
                }
            }
            return new ValidationResult(
                true,
                "Input options or tissue/detector combinations are valid",
                "");

        }
        /// <summary>
        /// Method checks SimulationInput against current in-capabilities of the code.
        /// </summary>
        /// <param name="input">SimulationInput</param>
        /// <returns>An instance of the ValidationResult class</returns>
        private static ValidationResult ValidateCurrentIncapabilities(SimulationInput input)
        {
            if (input.Options.AbsorptionWeightingType == AbsorptionWeightingType.Continuous)
            {
                foreach (var detectorInput in input.DetectorInputs)
                {
                    if (detectorInput.TallyDetails.IsNotImplementedForCAW)
                    {
                        return new ValidationResult(
                            false,
                            "The use of Continuous Absorption Weighting is not implemented for one of the infile detectors",
                            "Modify AbsorptionWeightingType to Discrete");
                    }
                }

            }
            if (input.Options.AbsorptionWeightingType == AbsorptionWeightingType.Discrete)
            {
                foreach (var detectorInput in input.DetectorInputs)
                {
                    if (detectorInput.TallyDetails.IsNotImplementedForDAW)
                    {
                        return new ValidationResult(
                            false,
                            "The use of Discrete Absorption Weighting with path length type detectors not implemented yet",
                            "Modify AbsorptionWeightingType to Continuous");
                    }
                }
            }
            foreach (var detectorInput in input.DetectorInputs)
            {
                // can only run dMC detectors with 1 perturbed region for the present
                if (detectorInput.TallyType.Contains("dMCdROfRhodMua"))
                {
                    return dMCdROfRhodMuaDetectorInputValidation.ValidateInput(detectorInput);
                }
                if (detectorInput.TallyType.Contains("dMCdROfRhodMus"))
                {
                    return dMCdROfRhodMusDetectorInputValidation.ValidateInput(detectorInput);
                }
                // check that number in blood volume list matches number of tissue subregions
                if (detectorInput.TallyType.Contains("ReflectedDynamicMTOfRhoAndSubregionHist"))
                {
                    return ReflectedDynamicMTOfRhoAndSubregionHistDetectorInputValidation.ValidateInput(detectorInput, input.TissueInput.Regions.Count());
                }
                if (detectorInput.TallyType.Contains("ReflectedDynamicMTOfXAndYAndSubregionHist"))
                {
                    return ReflectedDynamicMTOfXAndYAndSubregionHistDetectorInputValidation.ValidateInput(detectorInput, input.TissueInput.Regions.Count());
                }
                if (detectorInput.TallyType.Contains("TransmittedDynamicMTOfRhoAndSubregionHist"))
                {
                    return TransmittedDynamicMTOfRhoAndSubregionHistDetectorInputValidation.ValidateInput(detectorInput, input.TissueInput.Regions.Count());
                }
                if (detectorInput.TallyType.Contains("TransmittedDynamicMTOfXAndYAndSubregionHist"))
                {
                    return TransmittedDynamicMTOfXAndYAndSubregionHistDetectorInputValidation.ValidateInput(detectorInput, input.TissueInput.Regions.Count());
                }
                if (detectorInput.TallyType.Contains("SurfaceFiber"))
                {
                    return SurfaceFiberDetectorInputValidation.ValidateInput(detectorInput);
                }
                if (detectorInput.TallyType.Contains("SlantedRecessedFiber"))
                {
                    return SlantedRecessedFiberDetectorInputValidation.ValidateInput(detectorInput);
                }
                // check any ..RecessedDetectorInput 
                // this breaks form with prior checks that were on a detector basis
                if (detectorInput.TallyType.Contains("Recessed"))
                {
                    return RecessedDetectorInputValidation.ValidateInput(detectorInput);
                }
            }         
            return new ValidationResult(
                true,
                "Detector definitions are consistent with current capabilities");
        }
    }
}
