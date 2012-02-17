using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Vts.MonteCarlo.PostProcessing;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.IO;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.DataStructuresValidation;

namespace Vts.MonteCarlo.PostProcessor
{
    public class PostProcessorSetup
    {
        /// <summary>
        /// method to read the post processor input from a specified or default files
        /// </summary>
        public static PostProcessorInput ReadPostProcessorInputFromFile(string inputFile)
        {
            try
            {
                // read input file then read in elements of input file
                if (string.IsNullOrEmpty(inputFile))
                {
                        Console.WriteLine("\nNo input file specified. Using infile.xml from root mc_post.exe folder... ");
                        return ReadPostProcessorInputFromFile("infile.xml");
                }
            
                //get the full path for the input file
                var fullFilePath = Path.GetFullPath(inputFile);

                if (File.Exists(fullFilePath))
                {
                    return PostProcessorInput.FromFile(fullFilePath);
                }

                if (File.Exists(fullFilePath + ".xml"))
                {
                    return PostProcessorInput.FromFile(fullFilePath + ".xml");
                }

                //throw a file not found exception
                throw new FileNotFoundException("\nThe following input file could not be found: " + fullFilePath + " - type mc_post help=infile for correct syntax");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static ValidationResult ValidatePostProcessorInput(PostProcessorInput input)
        {
            return PostProcessorInputValidation.ValidateInput(input);
        }

        // need to work on following
        /// <summary>
        /// Runs the Monte Carlo Post-processor
        /// </summary>
        public static void RunPostProcessor(PostProcessorInput input, string outputFolderPath)
        {
            // locate root folder for output, creating it if necessary
            var path = string.IsNullOrEmpty(outputFolderPath)
                ? Path.GetFullPath(Directory.GetCurrentDirectory())
                : Path.GetFullPath(outputFolderPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // locate destination folder for output, creating it if necessary
            var resultsFolder = Path.Combine(path, input.OutputName);
            if (!Directory.Exists(resultsFolder))
            {
                Directory.CreateDirectory(resultsFolder);
            }

            Output postProcessedOutput = null;

            var databaseGenerationInputFile = SimulationInput.FromFile(Path.Combine(input.InputFolder, input.DatabaseSimulationInputFilename + ".xml"));
            // check for pMC tallies first because could have ReflectanceTallies mixed in and want to load CollisionInfo

            // Why not mirror the "on-the-fly" code, and allow for all kinds of detector inputs simultaneously? (dc 12/21/2011)
            if (input.DetectorInputs.Where(di => di.TallyType.IspMCReflectanceTally()).Any())
            {
                IList<IDetectorInput> pMCDetectorInputs;
                pMCDetectorInputs = input.DetectorInputs;
                //pMCDetectorInputs = input.DetectorInputs.Select(d => (IpMCDetectorInput)d).ToList();
                postProcessedOutput = PhotonDatabasePostProcessor.GenerateOutput(
                    VirtualBoundaryType.pMCDiffuseReflectance,
                    pMCDetectorInputs, 
                    input.TallySecondMoment,
                    input.TrackStatistics,
                    PhotonDatabaseFactory.GetpMCDatabase( // database filenames are assumed to be convention
                        VirtualBoundaryType.pMCDiffuseReflectance,
                        input.InputFolder),
                    databaseGenerationInputFile
                );
            }
            else if (input.DetectorInputs.Where(di => di.TallyType.IsReflectanceTally()).Any())
            {
                postProcessedOutput = PhotonDatabasePostProcessor.GenerateOutput(
                    VirtualBoundaryType.DiffuseReflectance,
                    input.DetectorInputs, 
                    input.TallySecondMoment,
                    PhotonDatabaseFactory.GetPhotonDatabase( //database filenames are assumed to be convention
                        VirtualBoundaryType.DiffuseReflectance,
                        input.InputFolder),
                    databaseGenerationInputFile
                );
            }
            else if (input.DetectorInputs.Where(di => di.TallyType.IsTransmittanceTally()).Any())
            {
                postProcessedOutput = PhotonDatabasePostProcessor.GenerateOutput(
                    VirtualBoundaryType.DiffuseTransmittance,
                    input.DetectorInputs,
                    input.TallySecondMoment,
                    PhotonDatabaseFactory.GetPhotonDatabase( //database filenames are assumed to be convention
                        VirtualBoundaryType.DiffuseTransmittance,
                        input.InputFolder),
                    databaseGenerationInputFile
                );
            }
            else if (input.DetectorInputs.Where(di => di.TallyType.IsSpecularReflectanceTally()).Any())
            {
                postProcessedOutput = PhotonDatabasePostProcessor.GenerateOutput(
                    VirtualBoundaryType.SpecularReflectance,
                    input.DetectorInputs,
                    input.TallySecondMoment,
                    PhotonDatabaseFactory.GetPhotonDatabase( //database filenames are assumed to be convention
                        VirtualBoundaryType.SpecularReflectance,
                        input.InputFolder),
                    databaseGenerationInputFile
                );
            }

            var folderPath = input.OutputName;
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // save input file to output folder with results
            input.ToFile(resultsFolder + "\\" + input.OutputName + ".xml");

            // save database generation input file to output folder
            databaseGenerationInputFile.ToFile(resultsFolder + "\\" + input.OutputName + "_database_infile.xml");

            if (postProcessedOutput != null)
            {
                foreach (var result in postProcessedOutput.ResultsDictionary.Values)
                {
                    // save all detector data to the specified folder
                    DetectorIO.WriteDetectorToFile(result, folderPath);
                }
            }
        }
    }
}


