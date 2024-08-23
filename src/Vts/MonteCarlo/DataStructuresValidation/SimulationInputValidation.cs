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
    public static class SimulationInputValidation
    {
        /// <summary>
        /// Master of call validation methods. Calls methods to validate source,
        /// tissue and detector definitions.
        /// </summary>
        /// <param name="input">SimulationInput to be validated</param>
        /// <returns>An instance of ValidationResult with IsValid bool set and message about error if false</returns>
        public static ValidationResult ValidateInput(SimulationInput input)
        {
            var validations = new[]
                {
                    si => ValidateN(si.N),
                    si => ValidateSourceInput(si.SourceInput, si.TissueInput),
                    si => ValidateTissueInput(si.TissueInput),
                    ValidateDetectorInput,
                    ValidateCombinedInputParameters,
                    ValidateCurrentCapabilities
                };

            foreach (var validation in validations)
            {
                var tempResult = validation(input);
                if (!tempResult.IsValid) return tempResult;
                
            }
            
            return new ValidationResult( true, "Simulation input is valid");

        }

        private static ValidationResult ValidateN(long n)
        {
            return new ValidationResult(
                n >= 10,
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

            return sourceInput switch
            {
                FluorescenceEmissionAOfRhoAndZSourceInput => FluorescenceEmissionAOfRhoAndZSourceInputValidation
                    .ValidateInput(sourceInput),
                FluorescenceEmissionAOfXAndYAndZSourceInput => FluorescenceEmissionAOfXAndYAndZSourceInputValidation
                    .ValidateInput(sourceInput),
                _ => new ValidationResult(true, "Starting photons in region " + sourceInput.InitialTissueRegionIndex)
            };
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

            return tissueInput switch
            {
                MultiLayerTissueInput => MultiLayerTissueInputValidation.ValidateInput(tissueInput),
                SingleEllipsoidTissueInput => SingleEllipsoidTissueInputValidation.ValidateInput(tissueInput),
                SingleVoxelTissueInput => SingleVoxelTissueInputValidation.ValidateInput(tissueInput),
                SingleInfiniteCylinderTissueInput => SingleInfiniteCylinderTissueInputValidation.ValidateInput(
                    tissueInput),
                MultiConcentricInfiniteCylinderTissueInput => MultiConcentricInfiniteCylinderTissueInputValidation
                    .ValidateInput(tissueInput),
                BoundingCylinderTissueInput => BoundingCylinderTissueInputValidation.ValidateInput(tissueInput),
                _ => new ValidationResult(true, "Tissue input must be valid",
                    "Validation skipped for tissue input " + tissueInput + ". No matching validation rules were found.")
            };
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
                        "DetectorInput not implemented yet:" + detectorInput,
                        "Please omit " + detectorInput + " from DetectorInput list");
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
        /// and source, tissue, detector definitions. The philosophy here is that if the transport will
        /// not error, a warning is issued and the validation result remains true.  This allows users to
        /// specify inconsistent combinations, e.g. angled source and cylindrical coordinate detectors,
        /// receive a warning and have the simulation proceed.  However, if the transport will error then
        /// the validation result will be false, the validationRule and remarks output and simulation stops,
        /// e.g. embedded ellipsoid in tissue that overlaps tissue layer. 
        /// </summary>
        /// <param name="input">input to be validated</param>
        /// <returns>An instance of ValidationResult with IsValid set and error message if false</returns>
        private static ValidationResult ValidateCombinedInputParameters(SimulationInput input)
        {
            // check that absorption weighting type set to analog and RR weight threshold != 0.0
            if (input.Options.AbsorptionWeightingType == AbsorptionWeightingType.Analog &&
                input.Options.RussianRouletteWeightThreshold > 0.0)
            {
                return new ValidationResult(
                    false,
                    "Russian Roulette cannot be employed with Analog absorption weighting is specified",
                    "With Analog absorption weighting, set Russian Roulette weight threshold = 0.0");
            }

            // check combination of tissue definition with detector definition
            var tempResult = ValidateTissueCombinedWithDetectors(input);
            if (!tempResult.IsValid) return tempResult;

            // check combination of source definition with detector definition
            if (input.SourceInput is DirectionalPointSourceInput source && 
                source.Direction != new Direction(0,0,1) && 
                input.DetectorInputs.Any(detectorInput => detectorInput.TallyDetails.IsCylindricalTally))
            {
                Console.WriteLine("Warning: Angled source and cylindrical coordinate detector defined: user discretion advised");
                return new ValidationResult(
                    true,
                    "Warning: Angled source and cylindrical coordinate detector defined",
                    "User discretion advised: change detector to Cartesian equivalent or define source to be normal");
            }
            if (input.DetectorInputs.Where(detectorInput => detectorInput.TallyDetails.IsTransmittanceTally && 
                input.TissueInput is MultiLayerTissueInput).Any(detectorInput => ((dynamic)detectorInput).FinalTissueRegionIndex == 0))
            {
                return new ValidationResult(
                    false,
                    "Transmittance detectors with MultiLayerTissues cannot detect in tissue region 0",
                    "Change FinalTissueRegionIndex to be index of air below tissue (index >= 2)");
            }
            return new ValidationResult(
                true,
                "Input options or tissue/detector combinations are valid",
                "");
        }

        private static ValidationResult ValidateTissueCombinedWithDetectors(SimulationInput input)
        {
            switch (input.TissueInput)
            {
                // check that if single ellipsoid tissue specified and (r,z) detector specified,
                // that (1) ellipsoid is centered at x=0, y=0, (2) ellipsoid is cylindrically symmetric (dx=dy)
                case SingleEllipsoidTissueInput tissueWithEllipsoid:
                {
                    var ellipsoid = (EllipsoidTissueRegion)tissueWithEllipsoid.EllipsoidRegion;
                    var tempResult = ValidateSingleEllipsoidTissueCombinedWithDetectors(input, ellipsoid);
                    if (!tempResult.IsValid) return tempResult;
                    break;
                }
                // check that if single voxel or single infinite cylinder tissue specified,
                // cannot specify (r,z) detector 
                case SingleVoxelTissueInput:
                case SingleInfiniteCylinderTissueInput:
                    {
                        foreach (var detectorInput in input.DetectorInputs)
                        {
                            if (detectorInput.TallyDetails.IsCylindricalTally)
                            {
                                Console.WriteLine("Warning: voxel in tissue with cylindrical detector defined: user discretion advised");
                                return new ValidationResult(
                                true,
                                "Warning: voxel in tissue with cylindrical detector defined",
                                "User discretion advised: change detector inputs to specify non-cylindrical type tallies");
                            }

                            if (detectorInput.TallyType != TallyType.ROfFx) continue;
                            Console.WriteLine("Warning: R(fx) theory assumes a homogeneous or layered tissue geometry: user discretion advised");
                            return new ValidationResult(
                                true,
                                "Warning: R(fx) theory assumes a homogeneous or layered tissue geometry",
                                "User discretion advised");
                        }
                        break;
                    }
                // check that if bounding volume tissue specified, the ATotalBoundingVolumeTissueInput detector needs
                // to be specified
                case BoundingCylinderTissueInput when input.DetectorInputs.All(d => d.TallyType != TallyType.ATotalBoundingVolume):
                    return new ValidationResult(
                    false,
                    "BoundingCylinderTissueInput needs associated detector ATotalBoundingVolume to be defined",
                    "Add ATotalBoundingVolumeDetectorInput to detector inputs");
            }
            return new ValidationResult(
                true,
                "Input options or tissue/detector combinations are valid",
                "");
        }

        private static ValidationResult ValidateSingleEllipsoidTissueCombinedWithDetectors(
            SimulationInput input, EllipsoidTissueRegion ellipsoid)
        {
            foreach (var detectorInput in input.DetectorInputs)
            {
                switch (detectorInput.TallyDetails.IsCylindricalTally)
                {
                    // check if ellipsoid off center, then not cylindrically symmetric, continue with warning
                    case true when
                        Math.Abs(ellipsoid.Center.X) > 1e-6 || Math.Abs(ellipsoid.Center.Y) > 1e-6:
                        Console.WriteLine("Warning: off center ellipsoid in tissue with cylindrical detector defined: user discretion advised");
                        return new ValidationResult(
                        true,
                        "Warning: off center ellipsoid in tissue with cylindrical detector defined",
                        "User discretion advised: change ellipsoid center to (0,0) or specify non-cylindrical type tally");
                    // check if Dx != Dy, then not cylindrically symmetric, continue with warning
                    case true when Math.Abs(ellipsoid.Dx - ellipsoid.Dy) > 1e-6:
                        Console.WriteLine("Warning: ellipsoid with Dx != Dy in tissue with cylindrical detector defined: user discretion advised");
                        return new ValidationResult(
                            true,
                            "Warning: ellipsoid with Dx != Dy in tissue with cylindrical detector defined",
                            "User discretion advised: change ellipsoid.Dx to be = to Dy or specify non-cylindrical type tally");
                }

                // theory assumes homogeneous tissue for R(fx), however users can continue with warning
                if (detectorInput.TallyType != TallyType.ROfFx) continue;
                Console.WriteLine("Warning: R(fx) theory assumes a homogeneous or layered tissue geometry: user discretion advised");
                return new ValidationResult(
                    true,
                    "Warning: R(fx) theory assumes a homogeneous or layered tissue geometry",
                    "User discretion advised");
            }
            return new ValidationResult(
                true,
                "Input options or tissue/detector combinations are valid",
                "");
        }

        /// <summary>
        /// Method checks SimulationInput against current capabilities of the code.
        /// </summary>
        /// <param name="input">SimulationInput</param>
        /// <returns>An instance of the ValidationResult class</returns>
        private static ValidationResult ValidateCurrentCapabilities(SimulationInput input)
        {
            switch (input.Options.AbsorptionWeightingType)
            {
                case AbsorptionWeightingType.Continuous when input.DetectorInputs.Any(detectorInput => detectorInput.TallyDetails.IsNotImplementedForCAW):
                    return new ValidationResult(
                        false,
                        "The use of Continuous Absorption Weighting is not implemented for one of the infile detectors",
                        "Modify AbsorptionWeightingType to Discrete");
                case AbsorptionWeightingType.Discrete:
                {
                    if (input.DetectorInputs.Any(detectorInput => detectorInput.TallyDetails.IsNotImplementedForDAW))
                    {
                        return new ValidationResult(
                            false,
                            "The use of Discrete Absorption Weighting with path length type detectors not implemented yet",
                            "Modify AbsorptionWeightingType to Continuous");
                    }

                    break;
                }
                case AbsorptionWeightingType.Analog:
                case AbsorptionWeightingType.Continuous:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(typeof(AbsorptionWeightingType).ToString());
            }
            // check current detector capabilities
            var tempResult = ValidateCurrentDetectorCapabilities(input);
            if (!tempResult.IsValid) return tempResult;

            return new ValidationResult(
                true,
                "Detector definitions are consistent with current capabilities");
        }

        private static ValidationResult ValidateCurrentDetectorCapabilities(
            SimulationInput input)
        {
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
                var tempResult = ValidateBloodVolumeDetectorConsistencies(input, detectorInput);
                if (!tempResult.IsValid) return tempResult;

                // check fiber consistencies
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

        private static ValidationResult ValidateBloodVolumeDetectorConsistencies(
            SimulationInput input, IDetectorInput detectorInput)
        {
            if (detectorInput.TallyType.Contains("ReflectedDynamicMTOfRhoAndSubregionHist"))
            {
                return ReflectedDynamicMTOfRhoAndSubregionHistDetectorInputValidation.ValidateInput(
                    detectorInput, input.TissueInput.Regions.Length);
            }
            if (detectorInput.TallyType.Contains("ReflectedDynamicMTOfXAndYAndSubregionHist"))
            {
                return ReflectedDynamicMTOfXAndYAndSubregionHistDetectorInputValidation.ValidateInput(
                    detectorInput, input.TissueInput.Regions.Length);
            }
            if (detectorInput.TallyType.Contains("TransmittedDynamicMTOfRhoAndSubregionHist"))
            {
                return TransmittedDynamicMTOfRhoAndSubregionHistDetectorInputValidation.ValidateInput(
                    detectorInput, input.TissueInput.Regions.Length);
            }
            if (detectorInput.TallyType.Contains("TransmittedDynamicMTOfXAndYAndSubregionHist"))
            {
                return TransmittedDynamicMTOfXAndYAndSubregionHistDetectorInputValidation.ValidateInput(
                    detectorInput, input.TissueInput.Regions.Length);
            }
            return new ValidationResult(
                true,
                "Detector definitions are consistent with current capabilities");

        }
    }
}
