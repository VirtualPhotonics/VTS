using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vts.MonteCarlo.DataStructuresValidation;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This class validates whether the fields in PostProcessorInput have been specified
    /// correctly or not.
    /// </summary>
    public class PostProcessorInputValidation
    {
        /// <summary>
        /// validate infile as well as overriding command line input
        /// </summary>
        /// <param name="input">PostProcessorInput file</param>
        /// <param name="inpath">command line option inpath</param>
        /// <returns></returns>
        public static ValidationResult ValidateInput(PostProcessorInput input, string inpath)
        {
            ValidationResult tempResult;
            tempResult = ValidateTissueOpticalProperties(input.DetectorInputs);
            if (!tempResult.IsValid)

            {
                return tempResult;
            }

            tempResult = ValidateInputFolderExistence(Path.Combine(inpath, input.InputFolder));
            if (!tempResult.IsValid)
            {
                return tempResult;
            }

            tempResult = ValidatePhotonDatabaseExistence(input.DetectorInputs, Path.Combine(inpath, input.InputFolder));
            if (!tempResult.IsValid)
            {
                return tempResult;
            }

            tempResult = ValidateSimulationInputExistence(input.DatabaseSimulationInputFilename + ".txt", 
                Path.Combine(inpath, input.InputFolder));
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
            if (detectorInputs.Any(di => di.TallyDetails.IsReflectanceTally))
            {
                return new ValidationResult(
                    File.Exists(Path.Combine(inputFolder, "DiffuseReflectanceDatabase")),
                    "PostProcessorInput:  file DiffuseReflectanceDatabase does not exist",
                    "check that VirtualBoundaryType and database type agree");
            }
            if (detectorInputs.Any(di => di.TallyDetails.IsTransmittanceTally))
            {
                return new ValidationResult(
                    File.Exists(Path.Combine(inputFolder, "DiffuseTransmittanceDatabase")),
                    "PostProcessorInput:  file DiffuseTransmittanceDatabase does not exist",
                    "check that VirtualBoundaryType and database type agree");
            }
            if (detectorInputs.Any(di => di.TallyDetails.IsSpecularReflectanceTally))
            {
                return new ValidationResult(
                    File.Exists(Path.Combine(inputFolder, "SpecularReflectanceDatabase")),
                    "PostProcessorInput:  file SpecularReflectanceDatabase does not exist",
                    "check that VirtualBoundaryType and database type agree");
            }
            if (detectorInputs.Any(di => di.TallyDetails.IspMCReflectanceTally)) //pMC uses same exit db as regular post-processing
            {
                return new ValidationResult(
                      File.Exists(Path.Combine(inputFolder, "DiffuseReflectanceDatabase")) &&
                      File.Exists(Path.Combine(inputFolder, "CollisionInfoDatabase")),
                      "PostProcessorInput:  files DiffuseReflectanceDatabase or CollisionInfoDatabase do not exist",
                      "check that VirtualBoundaryType and database type agree");
            }
            if (detectorInputs.Select(di => di.TallyDetails.IspMCTransmittanceTally).Any()) //pMC uses same exit db as regular post-processing
            {
                return new ValidationResult(
                    File.Exists(Path.Combine(inputFolder, "DiffuseTransmittanceDatabase")) &&
                    File.Exists(Path.Combine(inputFolder, "CollisionInfoDatabase")),
                    "PostProcessorInput:  files DiffuseTransmittanceDatabase or CollisionInfoDatabase do not exist",
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
                File.Exists(Path.Combine(inputFolder, simulationInputFilename)),
                "PostProcessorInput:  SimulationInput filename does not exist",
                "check that a SimulationInput file exists in inputFolder");
        }
        private static ValidationResult ValidateTissueOpticalProperties(
            IList<IDetectorInput> detectorInputs)
        {
            // for all pMC detectors, check that perturbed OPs are non-negative (g could be neg)
            foreach (var detectorInput in detectorInputs)
            {
                if (detectorInput.TallyDetails.IspMCReflectanceTally)
                {
                    var ops = ((dynamic)detectorInput).PerturbedOps;
                    foreach (var op in ops)
                    {
                        if ((op.Mua < 0.0) || (op.Musp < 0.0) || (op.N < 0.0))
                        {

                            return new ValidationResult(
                            false,
                             "Tissue optical properties mua, mus', n need to be non-negative",
                             "Please check optical properties");
                        }
                    }
                }
            }
            return new ValidationResult(
                true,
                "PostProcessorInput:  perturbed optical properties are all non-negative");
        }
    }
}
