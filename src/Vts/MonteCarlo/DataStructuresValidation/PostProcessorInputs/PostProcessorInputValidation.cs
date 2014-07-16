using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vts.MonteCarlo.DataStructuresValidation;
using Vts.MonteCarlo.Extensions;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This class validates whether the fields in PostProcessorInput have been specified
    /// correctly or not.
    /// </summary>
    public class PostProcessorInputValidation
    {
        public static ValidationResult ValidateInput(PostProcessorInput input)
        {
            ValidationResult tempResult;

            tempResult = ValidateInputFolderExistence(input.InputFolder);
            if (!tempResult.IsValid)
            {
                return tempResult;
            }

            tempResult = ValidatePhotonDatabaseExistence(input.DetectorInputs, input.InputFolder);
            if (!tempResult.IsValid)
            {
                return tempResult;
            }

            tempResult = ValidateSimulationInputExistence(input.DatabaseSimulationInputFilename, input.InputFolder);
            if (!tempResult.IsValid)
            {
                return tempResult;
            }

            return new ValidationResult(
                true,
                "PostProcessorinput: input must be valid");
        }
        private static ValidationResult ValidateInputFolderExistence(string inputFolder)
        {
            // check if results folder exists or not
            return new ValidationResult(
                Directory.Exists(inputFolder),
                "PostProcessorInput: the input folder does not exist",
                "check that the input folder name agrees with the folder name on system");            
        }
        private static ValidationResult ValidatePhotonDatabaseExistence(
            IList<IDetectorInput> detectorInputs, string inputFolder)
        {
            if (detectorInputs.Select(di => di.TallyDetails.IsReflectanceTally).Any())
            {
                return new ValidationResult(
                    File.Exists(Path.Combine(inputFolder, "DiffuseReflectanceDatabase")),
                    "PostProcessorInput:  file DiffuseReflanceDatabase does not exist",
                    "check that VirtualBoundaryType and database type agree");
            }
            if (detectorInputs.Select(di => di.TallyDetails.IsTransmittanceTally).Any())
            {
                return new ValidationResult(
                    File.Exists(Path.Combine(inputFolder, "DiffuseTransmittanceDatabase")),
                    "PostProcessorInput:  file DiffuseTransmittanceDatabase does not exist",
                    "check that VirtualBoundaryType and database type agree");
            }
            if (detectorInputs.Select(di => di.TallyDetails.IsSpecularReflectanceTally).Any())
            {
                return new ValidationResult(
                    File.Exists(Path.Combine(inputFolder, "SpecularReflectanceDatabase")),
                    "PostProcessorInput:  file SpecularReflectanceDatabase does not exist",
                    "check that VirtualBoundaryType and database type agree");
            }
            if (detectorInputs.Select(di => di.TallyDetails.IspMCReflectanceTally).Any()) //pMC uses same exit db as regular post-processing
            {
                return new ValidationResult(
                      File.Exists(Path.Combine(inputFolder, "DiffuseReflectanceDatabase")) &&
                      File.Exists(Path.Combine(inputFolder, "CollisionInfoDatabase")),
                      "PostProcessorInput:  files DiffuseReflectanceDatabase or CollisionInfoDatabase do not exist",
                      "check that VirtualBoundaryType and database type agree");
            }
            return new ValidationResult(
                true,
                "PostProcessor database exists");
        }
        private static ValidationResult ValidateSimulationInputExistence(
            string simulationInputFilename, string inputFolder)
        {
            return new ValidationResult(
                !File.Exists(Path.Combine(inputFolder, simulationInputFilename)),
                "PostProcessorInput:  SimulationInput filename does not exist",
                "check that a SimulationInput file exists in inputFolder");
        }
    }
}
