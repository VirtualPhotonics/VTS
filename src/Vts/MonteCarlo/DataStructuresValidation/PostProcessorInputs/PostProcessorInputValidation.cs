using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.DataStructuresValidation;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This class validates whether the fields in PostProcessorInput have been specified
    /// correctly or not.
    /// </summary>
    /// <param name="input"></param>
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

            tempResult = ValidatePhotonDatabaseExistence(input.VirtualBoundaryType, input.InputFolder);
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
                !Directory.Exists(inputFolder),
                "PostProcessorInput: the input folder does not exist",
                "check that the input folder name agrees with the folder name on system");            
        }
        private static ValidationResult ValidatePhotonDatabaseExistence(
            VirtualBoundaryType virtualBoundaryType, string inputFolder)
        {
            switch (virtualBoundaryType)
            {
                case VirtualBoundaryType.DiffuseReflectance:
                    return new ValidationResult(
                        !File.Exists(Path.Combine(inputFolder, "DiffuseReflectanceDatabase")),
                        "PostProcessorInput:  file DiffuseReflanceDatabase does not exist",
                        "check that VirtualBoundaryType and database type agree");
                case VirtualBoundaryType.DiffuseTransmittance:
                    return new ValidationResult(
                        !File.Exists(Path.Combine(inputFolder, "DiffuseTransmittanceDatabase")),
                        "PostProcessorInput:  file DiffuseReflanceDatabase does not exist",
                        "check that VirtualBoundaryType and database type agree");
                case VirtualBoundaryType.SpecularReflectance:
                    return new ValidationResult(
                        !File.Exists(Path.Combine(inputFolder, "SpecularReflectanceDatabase")),
                        "PostProcessorInput:  file DiffuseReflanceDatabase does not exist",
                        "check that VirtualBoundaryType and database type agree");
                case VirtualBoundaryType.pMCDiffuseReflectance: //pMC uses same exit db as regular post-processing
                    return new ValidationResult(
                          !File.Exists(Path.Combine(inputFolder, "DiffuseReflectanceDatabase")),
                          "PostProcessorInput:  file DiffuseReflanceDatabase does not exist",
                          "check that VirtualBoundaryType and database type agree");
                default:
                    return null;
            }
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
